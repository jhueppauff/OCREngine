using System;
using System.Collections.Generic;
using System.Text;

namespace OCREngine.Function.FileExtentionHandler
{
    public class ExtentionBase
    {
        protected string FileExtention { get; set; }

        protected string MimeType { get; set; }

        public List<string> ExceuteCustomFileAction(string fileExtention, string filePath)
        {
            switch (FileExtention)
            {
                case ".pdf":
                    PDFHandler handler = new PDFHandler();
                    return handler.GetDocumentPages(filePath);
                case ".jpg":
                    return new List<string>() { filePath };
                case ".png":
                    return new List<string>() { filePath };
                default:
                    return null;
            }
        }

        public virtual List<string> GetDocumentPages(string filePath)
        {
            return new List<string>() { new string(filePath) };
        }
    }
}
