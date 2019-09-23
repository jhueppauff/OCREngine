using System.Collections.Generic;
using System.Threading.Tasks;
using AzureStorageAdapter.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OCREngine.Application.Helper;
using OCREngine.Domain.Entities;
using OCREngine.Domain.Entities.Vision;
using OCREngine.DurableFunction.Model;

namespace OCREngine.DurableFunction
{
    public static class Orchestrator
    {
        private static TableStorageAdapter tableStorage = new TableStorageAdapter(EnviromentHelper.GetEnvironmentVariable("StorageConnectionString"));

        [FunctionName("ProcessDocument")]
        public static async Task<string> RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context)
        {
            OcrRequest request = context.GetInput<OcrRequest>();

            // Split PDF into Images
            List<string> images = await context.CallActivityAsync<List<string>>("SplitDocument", request.DownloadUrl);

            // Analyze Images
            List<OcrResult> ocrResults = new List<OcrResult>();
            foreach (var image in images)
            {
                ocrResults.Add(await context.CallActivityAsync<OcrResult>("ImageProcessor", image));
            }
            
            // Construction Request
            ConstructDocumentRequest constructDocumentRequest = new ConstructDocumentRequest()
            {
                Files = images,
                OcrResults = ocrResults
            };

            // Construct PDF
            string documentUrl = await context.CallActivityAsync<string>("ConstructDocument", constructDocumentRequest);

            request.BlobUri = documentUrl;

            return JsonConvert.SerializeObject(request);
        }

        [return: Table("DocumentRequests")]
        [FunctionName("QueueTrigger")]
        public static async Task<OcrRequest> Run([QueueTrigger("documentrequests", Connection = "StorageConnectionString")]string queueTrigger,
            [OrchestrationClient]DurableOrchestrationClient starter, ILogger log)
        {
            OcrRequest request = await tableStorage.RetrieveRecord<OcrRequest>("DocumentRequests", 
                new Microsoft.WindowsAzure.Storage.Table.TableEntity() { PartitionKey = "Documents", RowKey = queueTrigger })
                .ConfigureAwait(false);

            // Function input comes from the request content.
            var result = await starter.StartNewAsync("ProcessDocument", request);

            return JsonConvert.DeserializeObject<OcrRequest>(result);
        }
    }
}