using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using OCREngine.Application.Vision;
using OCREngine.Domain.Entities;

namespace OCREngine.Function
{
    public static class ImageProcessor
    {
        [return: Table("ImageRequests")]
        [FunctionName("ImageProcessor")]
        public static async Task<ImageRequest> Run([QueueTrigger("ImageQueue", Connection = "StorageConnectionString")]string queueTrigger, [Table("ImageRequests", "Images", "{queueTrigger}")] ImageRequest imageRequest, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {queueTrigger}");

            VisionServiceClient visionService = new VisionServiceClient(EnviromentHelper.GetEnvironmentVariable("VisionApiSubscriptionKey"), EnviromentHelper.GetEnvironmentVariable("VisionApiEndpoint"));
            var result = await visionService.RecognizeTextAsync(imageRequest.BlobUri);

            imageRequest.Response = result;

            return imageRequest;
        }
    }
}
