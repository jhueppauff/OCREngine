namespace OCREngine.Function
{
    using AzureStorageAdapter.Queue;
    using AzureStorageAdapter.Table;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Azure.WebJobs.Host;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Table;
    using OCREngine.Function.Clients;
    using OCREngine.Function.Entities;
    using OCREngine.Function.Vision;
    using OCREngine.Function.Vision.Models;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public static class DocumentProcessor
    {
        private const string tableName = "OcrProcessing";
        private const string queueName = "ocrprocessing";

        private static ILogger logger;

        /// <summary>
        /// Queues a new Document for Processing
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("QueueDocument")]
        public static async Task<HttpResponseMessage> QueueDocument([HttpTrigger(AuthorizationLevel.Function, "put", Route = null)]HttpRequestMessage req, ILogger logger)
        {
            DocumentProcessor.logger = logger;
            logger.LogInformation("Add new Document to queue started");

            if (req == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Request is null");
            }

            OcrRequest input = await req.Content.ReadAsAsync<OcrRequest>();

            if (input == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Request was not supplied in body");
            }

            var request = new OcrRequest(DateTime.Now.Year.ToString(), Guid.NewGuid().ToString())
            {
                ProcessingState = ProcessingStates.Queued.ToString(),
                DownloadUrl = input.DownloadUrl
            };

            await InsertRequestToStorage(request).ConfigureAwait(false);
            
            return req.CreateResponse(HttpStatusCode.OK, Guid.Parse(request.RowKey));
        }

        private static async Task InsertRequestToStorage(OcrRequest request)
        {
            string connectionString = EnviromentHelper.GetEnvironmentVariable("StorageConnectionString");
            QueueStorageAdapter queueStorageAdapter = new QueueStorageAdapter(connectionString);
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(connectionString);

            try
            {
                await tableStorageAdapter.CreateNewTable(tableName).ConfigureAwait(false);
                await tableStorageAdapter.InsertRecordToTable(tableName, request).ConfigureAwait(false);

                await queueStorageAdapter.CreateQueueAsync(queueName).ConfigureAwait(false);
                await queueStorageAdapter.AddEntryToQueueAsync(queueName, request.RowKey).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.LogError(exception: ex, message: "An Error Occured while saving to Storage");
                throw;
            }
        }

        /// <summary>
        /// Main Processing function
        /// </summary>
        /// <returns></returns>
        [FunctionName("ProcessDocument")]
        public static async Task ProcessDocument([QueueTrigger(queueName)]string queueItem, ILogger logger)
        {
            if (string.IsNullOrEmpty(queueItem))
            {
                throw new ArgumentException("message", nameof(queueItem));
            }

            string connectionString = EnviromentHelper.GetEnvironmentVariable("StorageConnectionString");
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(connectionString);

            OcrRequest requestData = await tableStorageAdapter.RetrieveRecord<OcrRequest>(tableName, new TableEntity() { PartitionKey = DateTime.Now.Year.ToString(), RowKey = queueItem }).ConfigureAwait(false);
            requestData.ProcessingState = ProcessingStates.InProgress.ToString();

            await tableStorageAdapter.InsertRecordToTable(tableName, requestData).ConfigureAwait(false);

            FileExtentionHandler.ExtentionBase extentionBase = new FileExtentionHandler.ExtentionBase();

            List<string> files = extentionBase.ExceuteCustomFileAction(DownloadFile(requestData.DownloadUrl));

            VisionServiceClient visionService = new VisionServiceClient(EnviromentHelper.GetEnvironmentVariable("VisionApiSubscriptionKey"), EnviromentHelper.GetEnvironmentVariable("VisionApiEndpoint"));

            List<OcrResults> ocrResults = new List<OcrResults>();
            foreach (string file in files)
            {
                var result = await visionService.RecognizeTextAsync(File.OpenRead(file));

                ocrResults.Add(result);
            }
            
        }

        private static string DownloadFile(string downloadPath)
        {
            string path = Path.GetTempPath();
            FileWebClient client = new FileWebClient();

            string fileName = client.DownloadFile(path, downloadPath);

            return fileName;
        }

        /// <summary>
        /// Returns the Process State of a Document
        /// </summary>
        /// <param name="null"></param>
        /// <returns></returns>
        [FunctionName("GetProcessStatus")]
        public static async Task<HttpResponseMessage> GetProcessStatus([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger logger)
        {
            string connectionString = EnviromentHelper.GetEnvironmentVariable("StorageConnectionString");

            if (req == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            OcrRequest input = await req.Content.ReadAsAsync<OcrRequest>();

            TableStorageAdapter storageAdapter = new TableStorageAdapter(connectionString);
            var result = await storageAdapter.RetrieveRecord<OcrRequest>(tableName, input).ConfigureAwait(false);

            return req.CreateResponse(HttpStatusCode.OK, result.ProcessingState);
        }

        /// <summary>
        /// Gets the document download blob uri
        /// </summary>
        /// <param name="null"></param>
        /// <returns></returns>
        [FunctionName("GetDocument")]
        public static async Task<HttpResponseMessage> GetDocument([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, ILogger log)
        {
            string connectionString = EnviromentHelper.GetEnvironmentVariable("StorageConnectionString");

            if (req == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            OcrRequest input = await req.Content.ReadAsAsync<OcrRequest>();

            TableStorageAdapter storageAdapter = new TableStorageAdapter(connectionString);
            var result = await storageAdapter.RetrieveRecord<OcrRequest>(tableName, input).ConfigureAwait(false);

            return req.CreateResponse(HttpStatusCode.OK, result.BlobUri);
        }

    }
}