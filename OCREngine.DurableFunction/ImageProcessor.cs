namespace OCREngine.DurableFunction
{
    using System.Threading.Tasks;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Extensions.Logging;
    using OCREngine.Application.Helper;
    using OCREngine.Application.Vision;
    using OCREngine.Domain.Entities.Vision;

    public static class ImageProcessor
    {
        [FunctionName("ImageProcessor")]
        public static async Task<OcrResult> Run([ActivityTrigger] string url, ILogger log)
        {
            VisionServiceClient visionService = new VisionServiceClient(EnviromentHelper.GetEnvironmentVariable("VisionApiSubscriptionKey"), EnviromentHelper.GetEnvironmentVariable("VisionApiEndpoint"));
            var result = await visionService.RecognizeTextAsync(url);

            return result;
        }
    }
}
