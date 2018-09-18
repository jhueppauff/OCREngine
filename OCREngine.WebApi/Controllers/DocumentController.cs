using System;
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

namespace OCREngine.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : Controller
    {
        private readonly IConfiguration configuration;

        public DocumentController(IConfiguration configuration)
        {
            this.configuration = configuration;
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

        [Route("BuildDocument")]
        [HttpPost(Name = "BuildDocument"), DisableRequestSizeLimit]
        public ActionResult BuildDocument([FromBody] OcrResults[] json)
        {
            HtmlParser htmlParser = new HtmlParser();
            List<HtmlDocument> documents = new List<HtmlDocument>();

            foreach (OcrResults result in json)
            {
                 documents.Add(htmlParser.CreateHtmlFromVisionResult(result));
            }
            
            return null;
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