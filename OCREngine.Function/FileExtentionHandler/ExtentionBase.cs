using Microsoft.Azure.WebJobs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OCREngine.Function.FileExtentionHandler
{
    public class ExtentionBase
    {
        protected string FileExtention { get; set; }

        protected string MimeType { get; set; }

        public List<string> ExceuteCustomFileAction(string filePath, ExecutionContext context = null)
        {
            switch (Path.GetExtension(filePath))
            {
                case ".pdf":
                    PDFHandler handler = new PDFHandler();
                    return handler.GetDocumentPages(filePath, context);
                case ".jpg":
                    return new List<string>() { filePath };
                case ".png":
                    return new List<string>() { filePath };
                default:
                    return null;
            }
        }

        public virtual List<string> GetDocumentPages(string filePath, ExecutionContext context = null)
        {
            return new List<string>() { new string(filePath) };
        }
    }
}
