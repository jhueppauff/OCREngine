using Microsoft.Azure.WebJobs;
using PdfiumViewer;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace OCREngine.Function.FileExtentionHandler
{
    internal class PDFHandler : ExtentionBase
    {
        public PDFHandler()
        {
            base.MimeType = "application/pdf";
            base.FileExtention = ".pdf";
        }   

        public override List<string> GetDocumentPages(string documentPath, ExecutionContext context)
        {
            AssemblyLoader.Preload(context, "pdfium");

            Guid instanceId = Guid.NewGuid();
            List<string> splittedFiles = new List<string>();

            using (PdfDocument document = PdfDocument.Load(documentPath))
            {
                for (int i = 0; i < document.PageCount; i++)
                {
                    string imagePath = Path.Combine(Path.GetTempPath(), instanceId + i.ToString() + ".jpeg");

                    var image = document.Render(i, 210, 297, 300, 300, PdfRotation.Rotate0, PdfRenderFlags.CorrectFromDpi);

                    image.Save(imagePath, ImageFormat.Jpeg);

                    splittedFiles.Add(imagePath);
                }
            }

            return splittedFiles;
        }
    }
}
