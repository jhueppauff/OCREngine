using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OCREngine.Domain.Entities;
using AzureStorageAdapter.Table;
using OCREngine.Application.Helper;
using AzureStorageAdapter.Queue;

namespace OCREngine.DurableFunction
{
    public static class QueueDocument
    {
        private static readonly TableStorageAdapter tableStorage = new TableStorageAdapter(EnviromentHelper.GetEnvironmentVariable("StorageConnectionString"));
        private static readonly QueueStorageAdapter queueStorage = new QueueStorageAdapter(EnviromentHelper.GetEnvironmentVariable("StorageConnectionString"));

        [FunctionName("QueueDocument")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            OcrRequest ocrRequest = new OcrRequest()
            {
                DownloadUrl = data.DownloadUrl,
                PartitionKey = "Documents",
                RowKey = Guid.NewGuid().ToString(),
                ProcessingState = "queued"
            };

            await tableStorage.InsertRecordToTable<OcrRequest>("DocumentRequests", ocrRequest);
            await queueStorage.AddEntryToQueueAsync("documentrequests", ocrRequest.RowKey);

            return (ActionResult)new OkObjectResult(ocrRequest);
        }
    }
}
