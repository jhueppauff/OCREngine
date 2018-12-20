using AzureStorageAdapter.Queue;
using AzureStorageAdapter.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using OCREngine.Function.Entities;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace OCREngine.Function
{
    public static class DocumentProcessor
    {
        private const string tableName = "OcrProcessing";
        private const string queueName = "OcrProcessing";

        /// <summary>
        /// Queues a new Document for Processing
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("QueueDocument")]
        public static async Task<HttpResponseMessage> QueueDocument([HttpTrigger(AuthorizationLevel.Function, "put", Route = null)]HttpRequestMessage req, ILogger logger)
        {
            logger.LogInformation("Add new Document to queue started");

            if (req == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Request is null");
            }

            string connectionString = EnviromentHelper.GetEnvironmentVariable("StorageConnectionString");
            QueueStorageAdapter queueStorageAdapter = new QueueStorageAdapter(connectionString);
            TableStorageAdapter tableStorageAdapter = new TableStorageAdapter(connectionString);

            OcrRequest input = await req.Content.ReadAsAsync<OcrRequest>();

            if (input == null)
            {
                return req.CreateResponse(HttpStatusCode.BadRequest, "Request was not supplied in body");
            }

            var requestId = Guid.NewGuid();
            var request = new OcrRequest()
            {
                PartitionKey = input.PartitionKey = DateTime.Now.Year.ToString(),
                BlobUri = input.BlobUri,
                RequestId = requestId
            };

            await tableStorageAdapter.InsertRecordToTable(tableName, request);
            await queueStorageAdapter.AddEntryToQueueAsync(queueName, requestId.ToString());

            return req.CreateResponse(HttpStatusCode.OK, request.RequestId);
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