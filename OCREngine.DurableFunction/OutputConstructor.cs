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
using OCREngine.Application.Document;
using HtmlAgilityPack;
using DinkToPdf;
using OCREngine.Application;
using OCREngine.Application.Clients.Output.PdfConverter;
using OCREngine.Application.Storage;
using OCREngine.Application.Helper;
using AzureStorageAdapter.Blob;
using OCREngine.DurableFunction.Model;

namespace OCREngine.DurableFunction
{
    public static class OutputConstructor
    {
        [FunctionName("ConstructDocument")]
        public static async Task<string> Run([ActivityTrigger] ConstructDocumentRequest request, ILogger log, ExecutionContext context)
        {
            

            HtmlParser htmlParser = new HtmlParser();
            List<HtmlDocument> htmlDocuments = new List<HtmlDocument>();
            int i = 0;
            foreach (var ocrResult in request.OcrResults)
            {
                htmlDocuments.Add(htmlParser.CreateHtmlFromVisionResult(ocrResult, request.Files[i]));
                i++;
            }

            AssemblyLoader.Preload(context, "libwkhtmltox");
            var converter = new SynchronizedConverter(new PdfTools());

            PdfClient pdfClient = new PdfClient(converter);

            string localPath = pdfClient.ConvertHtmlToPdf(htmlDocuments);

            // Upload to Blob
            BlobStorageAdapter blobStorageAdapter = new BlobStorageAdapter(EnviromentHelper.GetEnvironmentVariable("StorageConnectionString"));
            string url = await blobStorageAdapter.UploadToBlob(await File.ReadAllBytesAsync(localPath).ConfigureAwait(false), Path.GetFileName(localPath), "application/pdf", "documents", true);

            return url;
        }
    }
}
