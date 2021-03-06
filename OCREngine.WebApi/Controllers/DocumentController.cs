﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using OCREngine.WebApi.Vision;
using OCREngine.WebApi.Vision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OCREngine.WebApi.Document;
using HtmlAgilityPack;
using System.Net.Http;
using DinkToPdf.Contracts;

namespace OCREngine.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : Controller
    {
        private readonly IConfiguration configuration;
        private readonly IConverter converter;

        public DocumentController(IConfiguration configuration, IConverter converter)
        {
            this.configuration = configuration;
            this.converter = converter;
        }

        // POST api/values
        [Route("ProcessDocument")]
        [HttpPost(Name = "ProcessDocument"), DisableRequestSizeLimit]
        public async Task<ActionResult> ProcessDocument()
        {
            var watch = new Stopwatch();
            watch.Start();

            Vision.Models.Document document = new Vision.Models.Document();
            OcrResults results;
            // Get File
            try
            {
                document.FileLocation = GetPostedFile();
            }
            catch (Exception ex)
            {
                return Json("Upload Failed: " + ex.Message);
            }

            if (string.IsNullOrEmpty(document.LanguageCode))
            {
                document.LanguageCode = "unk";
            }

            try
            {
                FileStream stream = new FileStream(path: document.FileLocation, mode: FileMode.Open, access: FileAccess.Read);
                var FileInfo = new FileInfo(document.FileLocation);

                VisionServiceClient visionService = new VisionServiceClient(configuration.GetValue<string>("VisionApiSubscriptionKey"), configuration.GetValue<string>("VisionApiEndpoint"));

                results = await visionService.RecognizeTextAsync(imageStream: stream, languageCode: document.LanguageCode);



                stream.Close();
                stream.Dispose();

                watch.Stop();


                ApplicationInsightsHelper aiHelper = new ApplicationInsightsHelper(configuration);

                var metric = new Dictionary<string, double>
                {
                    ["ProcessTime(ms)"] = watch.ElapsedMilliseconds,
                    ["DocumentSize(bytes)"] = FileInfo.Length
                };

                var properties = new Dictionary<string, string>()
                {
                    ["FileType"] = FileInfo.Extension
                };

                aiHelper.TrackEvent("OCRProcessing", properties, metric);
            }
            catch (Exception ex)
            {
                return Json("Processing Failed: " + ex.Message);
                throw;
            }


            return Json(results);
        }

        [Route("ConvertPdfToImage")]
        [HttpPost(Name = "ConvertPdfToImage"), DisableRequestSizeLimit]
        public async Task<IActionResult> ConvertPdfToImage()
        {
            string path = GetPostedFile();


            return null;
        }

        [Route("BuildDocument")]
        [HttpPost(Name = "BuildDocument"), DisableRequestSizeLimit]
        public IActionResult BuildDocument([FromBody] OcrResults[] json)
        {
            try
            {
                HtmlParser htmlParser = new HtmlParser();
                List<HtmlDocument> documents = new List<HtmlDocument>();

                foreach (OcrResults result in json)
                {
                    documents.Add(htmlParser.CreateHtmlFromVisionResult(result));
                }

                Document.PdfConverter.PdfClient pdfClient = new Document.PdfConverter.PdfClient(converter);
                string path = pdfClient.ConvertHtmlToPdf(documents);

                MemoryStream ms = new MemoryStream();
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    stream.CopyTo(ms);
                }
                ms.Position = 0;

                System.IO.File.Delete(path);

                FileStreamResult streamResult = new FileStreamResult(ms, "application/pdf")
                {
                    FileDownloadName = "OCRResult.pdf"
                };

                return streamResult;
            }
            catch (Exception ex)
            {

                throw;
            }
            
        }


        private string GetPostedFile()
        {
            var file = Request.Form.Files[0];

            string path = Path.GetTempFileName();

            if (file.Length > 0)
            {
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

                return path;
            }
            else
            {
                return null;
            }
        }

    }
}