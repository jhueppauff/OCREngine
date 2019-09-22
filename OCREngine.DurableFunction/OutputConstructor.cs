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
    public static class OutputConstructor
    {
        [FunctionName("ConstructDocument")]
        public static async Task<string> Run([ActivityTrigger] List<OcrResult> results, ILogger log)
        {
            string url = string.Empty;

            // ToDo Build Document

            return url;
        }
    }
}
