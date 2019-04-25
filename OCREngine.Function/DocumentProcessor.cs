namespace OCREngine.Function
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using OCREngine.Domain.Entities;
    using AzureStorageAdapter.Queue;
    using AzureStorageAdapter.Table;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;
    using Microsoft.WindowsAzure.Storage.Table;
    using OCREngine.Function.Clients;
    using OCREngine.Function.Vision;
    using OCREngine.Domain.Entities.Vision;
    using Microsoft.WindowsAzure.Storage.Queue;

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
        public static async Task<HttpResponseMessage> QueueDocument([HttpTrigger(AuthorizationLevel.Function, "put", Route = null)][Table(tableName, Connection = "StorageConnectionString")]CloudTable tableOut, [Queue(queueName, Connection = "StorageConnectionString")]CloudQueue queueOut, HttpRequestMessage req, ILogger logger)
        {
            if (tableOut == null)
            {
                throw new ArgumentNullException(nameof(tableOut));
            }

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

            TableOperation tableOperation = TableOperation.Insert(request);
            await tableOut.ExecuteAsync(tableOperation).ConfigureAwait(false);
            await queueOut.AddMessageAsync(new CloudQueueMessage(request.RowKey)).ConfigureAwait(false);

            return req.CreateResponse(HttpStatusCode.OK, Guid.Parse(request.RowKey));
        }

        /// <summary>
        /// Main Processing function
        /// </summary>
        /// <returns></returns>
        [FunctionName("ProcessDocument")]
        public static async Task ProcessDocument([QueueTrigger(queueName)]string queueItem, ILogger logger, CloudTable inputTable, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            if (string.IsNullOrEmpty(queueItem))
            {
                throw new ArgumentException("message", nameof(queueItem));
            }

            string connectionString = EnviromentHelper.GetEnvironmentVariable("StorageConnectionString");
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(connectionString);

            TableOperation tableOperation = TableOperation.Retrieve<OcrRequest>(DateTime.Now.Year.ToString(), queueItem);
            var result = await inputTable.ExecuteAsync(tableOperation);

            
            OcrRequest requestData = await tableStorageAdapter.RetrieveRecord<OcrRequest>(tableName, new TableEntity() { , RowKey = queueItem }).ConfigureAwait(false);
            requestData.ProcessingState = ProcessingStates.InProgress.ToString();

            await tableStorageAdapter.InsertRecordToTable(tableName, requestData).ConfigureAwait(false);

            List<string> files = new List<string>();
            try
            {
                FileExtentionHandler.ExtentionBase extentionBase = new FileExtentionHandler.ExtentionBase();

                files = extentionBase.ExceuteCustomFileAction(await DownloadFile(requestData.DownloadUrl).ConfigureAwait(false), context);


                List<OcrResults> ocrResults = new List<OcrResults>();

                VisionServiceClient visionService = new VisionServiceClient(EnviromentHelper.GetEnvironmentVariable("VisionApiSubscriptionKey"), EnviromentHelper.GetEnvironmentVariable("VisionApiEndpoint"));


                foreach (string file in files)
                {
                    using (var fileStream = File.OpenRead(file))
                    {
                        var result = await visionService.RecognizeTextAsync(fileStream);
                        ocrResults.Add(result);
                    }
                }

                // Process OCR Results
                Output.Processor processor = new Output.Processor();

                string path = processor.BuildDocumentFromOcrResult(files, ocrResults, context);

                AzureStorageAdapter.Blob.BlobStorageAdapter blobStorageAdapter = new AzureStorageAdapter.Blob.BlobStorageAdapter(connectionString);
                await blobStorageAdapter.UploadToBlob(await File.ReadAllBytesAsync(path).ConfigureAwait(false), Path.GetFileName(path), "application/pdf", "ocruploads", true);

            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while processing document");
                requestData.ExceptionMessage = ex.Message;
                requestData.ProcessingState = ProcessingStates.Failed.ToString();
            }
            finally
            {
                foreach (string file in files)
                {
                    File.Delete(file);
                }
            }
        }

        private static async Task<string> DownloadFile(string downloadPath)
        {
            string path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            
           FileWebClient client = new FileWebClient();

            string fileName = await client.DownloadFile(path, downloadPath).ConfigureAwait(false);

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