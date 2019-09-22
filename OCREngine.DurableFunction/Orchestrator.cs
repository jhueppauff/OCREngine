using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OCREngine.Domain.Entities;
using OCREngine.Domain.Entities.Vision;

namespace OCREngine.DurableFunction
{
    public static class Orchestrator
    {
        [FunctionName("ProcessDocument")]
        public static async Task<string> RunOrchestrator([OrchestrationTrigger] DurableOrchestrationContext context, OcrRequest request)
        {
            // Split PDF into Images
            List<string> images = await context.CallActivityAsync<List<string>>("SplitDocument", request.DownloadUrl);

            // Analyze Images
            List<OcrResult> ocrResults = new List<OcrResult>();
            foreach (var image in images)
            {
                ocrResults.Add(await context.CallActivityAsync<OcrResult>("ImageProcessor", image));
            }

            // Construct PDF
            string documentUrl = await context.CallActivityAsync<string>("ConstructDocument", ocrResults);

            request.BlobUri = documentUrl;

            return JsonConvert.SerializeObject(request);
        }

        [return: Table("Requests")]
        [FunctionName("QueueTrigger")]
        public static async Task<OcrRequest> Run([QueueTrigger("ImageQueue", Connection = "StorageConnectionString")]string queueTrigger,
            [OrchestrationClient]DurableOrchestrationClient starter, [Table("Requests", "Documents", "{queueTrigger}")] OcrRequest request,
            ILogger log)
        {
            // Function input comes from the request content.
            var result = await starter.StartNewAsync("ProcessDocument", null, request);

            return JsonConvert.DeserializeObject<OcrRequest>(result);
        }
    }
}