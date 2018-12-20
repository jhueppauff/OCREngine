using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;
using System.Net;
using System.Net.Http;
using System.Collections.Generic;
using System;
using OCREngine.Function.Entities;
using AzureStorageAdapter.Table;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;

namespace OCREngine.Function
{
    public static class QueueDocument
    {
        private const string tableName = "OcrProcessing";

        /// <summary>
        /// Queues a new Document for Processing
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("QueueDocument")]
        public static async Task<HttpResponseMessage> RunQueueDocument([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, CloudTable tableOut, ILogger log)
        {
            log.LogInformation("Add new Document to queue started");

            if (req == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            OcrRequest input = await req.Content.ReadAsAsync<OcrRequest>();

            if (input == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }

            var requestId = Guid.NewGuid();
            var request = new OcrRequest()
            {
                PartitionKey = input.PartitionKey = DateTime.Now.Year.ToString(),
                BlobUri = input.BlobUri,
                RequestId = requestId
            };

            var multiAdd = TableOperation.Insert(request);
            await tableOut.ExecuteAsync(multiAdd);

            return req.CreateResponse(HttpStatusCode.OK, request);
        }
    
        /// <summary>
        /// Main Processing function
        /// </summary>
        /// <returns></returns>
        [FunctionName("ProcessDocument")]
        public static async Task<HttpResponseMessage> ProcessDocument([QueueTrigger(tableName)] string input, ILogger logger)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("A Parameter is null or empty ", nameof(input));
            }

            return null;
        }

        /// <summary>
        /// Returns the Process State of a Document
        /// </summary>
        /// <param name="null"></param>
        /// <returns></returns>
        [FunctionName("GetProcessStatus")]
        public static async Task<HttpResponseMessage> GetProcessStatus([HttpTrigger(AuthorizationLevel.Function, "post", Route = null)]HttpRequestMessage req, CloudTable tableOut, ILogger logger)
        {
            string connectionString = EnviromentHelper.GetEnvironmentVariable("StorageConnectionString");
            
            if (req == null)
            {
                return req.CreateResponse(HttpStatusCode.NotFound);
            }
            
            OcrRequest input = await req.Content.ReadAsAsync<OcrRequest>();

            TableStorageAdapter storageAdapter = new TableStorageAdapter(connectionString);
            var result  = await storageAdapter.RetrieveRecord<OcrRequest>(tableName, input).ConfigureAwait(false);

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
            var result  = await storageAdapter.RetrieveRecord<OcrRequest>(tableName, input).ConfigureAwait(false);

            return req.CreateResponse(HttpStatusCode.OK, result.BlobUri);
        }

    }
}