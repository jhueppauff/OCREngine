using System;
using System.Collections.Generic;
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

        public override List<string> GetDocumentPages(string filePath)
        {
            return null;
        }
    }
}
