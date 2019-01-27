using Ghostscript.NET;
using Ghostscript.NET.Rasterizer;
using Microsoft.Azure.WebJobs;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing.Imaging;

namespace OCREngine.Function.FileExtentionHandler
{
    public class PDFHandler : ExtentionBase
    {
        public PDFHandler()
        {
            base.MimeType = "application/pdf";
            base.FileExtention = ".pdf";
        }   

        public override List<string> GetDocumentPages(string documentPath, ExecutionContext context)
        {
            int desired_x_dpi = 96;
            int desired_y_dpi = 96;

            AssemblyLoader.Preload(context, "Ghostscript.NET.dll");

            Guid instanceId = Guid.NewGuid();
            List<string> splittedFiles = new List<string>();

            using (GhostscriptRasterizer rasterizer = new GhostscriptRasterizer())
            {
                rasterizer.Open(documentPath);

                for (int pageNumber = 1; pageNumber <= rasterizer.PageCount; pageNumber++)
                {
                    string imagePath = Path.Combine(Path.GetTempPath(), instanceId + pageNumber.ToString() + ".png");

                    var img = rasterizer.GetPage(desired_x_dpi, desired_y_dpi, pageNumber);

                    img.Save(imagePath, ImageFormat.Png);

                    splittedFiles.Add(imagePath);
                }

            }




            //using (PdfDocument document = PdfDocument.Load(documentPath))
            //{
            //    for (int i = 0; i < document.PageCount; i++)
            //    {
            //        

            //        //Image image = new Image();

            //        //image = document.Render(i, 210, 297, 300, 300, PdfRotation.Rotate0, PdfRenderFlags.CorrectFromDpi);

            //        //image.Save(imagePath, ImageFormat.Jpeg);

            //        //splittedFiles.Add(imagePath);
            //    }
            //}

            return splittedFiles;
        }
    }
}
