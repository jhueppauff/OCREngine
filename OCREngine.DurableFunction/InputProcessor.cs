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
using OCREngine.Application.Helper;
using PdfiumViewer;
using System.Drawing;
using System.Drawing.Imaging;
using AzureStorageAdapter.Blob;

namespace OCREngine.DurableFunction
{
    public static class InputProcessor
    {
        private static readonly BlobStorageAdapter blobStorage = new BlobStorageAdapter(EnviromentHelper.GetEnvironmentVariable("StorageConnectionString"));

        [FunctionName("SplitDocument")]
        public static async Task<List<string>> Run([ActivityTrigger] string url, ILogger log, ExecutionContext context)
        {
            string path = await FileHelper.DownloadFile(url).ConfigureAwait(false);
            List<string> splittedFiles = new List<string>();

            using (PdfDocument document = PdfDocument.Load(path))
            {

                string instanceId = Guid.NewGuid().ToString();

                for (int i = 0; i < document.PageCount; i++)
                {
                    Image image = document.Render(i, 210, 297, 300, 300, PdfRotation.Rotate0, PdfRenderFlags.CorrectFromDpi);
                    string imagePath = Path.Combine(Path.GetTempPath(), instanceId + i.ToString() + ".png");

                    image.Save(imagePath, ImageFormat.Jpeg);

                    splittedFiles.Add(await blobStorage.UploadToBlob(await File.ReadAllBytesAsync(imagePath).ConfigureAwait(false), Path.GetFileName(imagePath), "image/png", "documents", true));
                }
            }

            return splittedFiles;
        }
    }
}