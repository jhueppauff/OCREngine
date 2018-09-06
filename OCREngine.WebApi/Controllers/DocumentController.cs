using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OCREngine.WebApi.Models;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;

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
        [HttpPost, DisableRequestSizeLimit]
        public async Task<ActionResult> ProcessDocument([FromBody] Document document)
        {
            if (document != null)
            {
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
                
                VisionServiceClient visionService = new VisionServiceClient(configuration.GetValue<string>("VisionApiSubscriptionKey"), configuration.GetValue<string>("VisionApiEndpoint"));

                OcrResults results = await visionService.RecognizeTextAsync(document.FileLocation, document.LanguageCode);

                return Json(results);
            }
            else
            {
                return Json("Missing Body");
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