using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using OCREngine.Domain.Entities.Vision;
using System.Collections.Generic;

namespace OCREngine.DurableFunction
{
    public static class InputProcessor
    {
        [FunctionName("SplitDocument")]
        public static async Task<List<OcrResult>> Run([ActivityTrigger] string url, ILogger log)
        {
            // ToDo Split Document, Docker?
            List<OcrResult> results = new List<OcrResult>();

            return results;
        }
    }
}
