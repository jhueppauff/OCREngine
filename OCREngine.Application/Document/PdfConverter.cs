using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using DinkToPdf;
using DinkToPdf.Contracts;
using HtmlAgilityPack;
using 

namespace OCREngine.Application.Document
{
    /// <summary>
    /// PDF Converter
    /// </summary>
    public class PdfConverter
    {
        /// <summary>
        /// The converter
        /// </summary>
        private readonly IConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfConverter"/> class.
        /// </summary>
        /// <param name="converter">The converter.</param>
        public PdfConverter(IConverter converter)
        {
            this.converter = converter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfConverter"/> class.
        /// </summary>
        public PdfConverter()
        {
        }

        /// <summary>
        /// Converts the PDF to image.
        /// </summary>
        /// <param name="documentPath">The document path.</param>
        /// <returns>Returns a<see cref="List{String}"/> with the images</returns>
        public List<string> ConvertPdfToImage(string documentPath)
        {
            Guid instanceId = Guid.NewGuid();
            List<string> splittedFile = new List<string>();

            using (PdfDocument document = PdfDocument.Load(documentPath))
            {
                for (int i = 0; i < document.PageCount; i++)
                {
                    string imagePath = Path.Combine(Path.GetTempPath(), instanceId + i.ToString() + ".jpeg");

                    var image = document.Render(i, 210, 297, 300, 300, PdfRotation.Rotate0, PdfRenderFlags.CorrectFromDpi);

                    image.Save(imagePath, ImageFormat.Jpeg);

                    splittedFile.Add(imagePath);
                }
            }

            return splittedFile;
        }

        /// <summary>
        /// Converts the HTML to PDF.
        /// </summary>
        /// <param name="documents">The documents.</param>
        /// <returns>Returns a path to the file as <see cref="String"/></returns>
        public string ConvertHtmlToPdf(List<HtmlDocument> documents)
        {
            string path = Path.GetTempFileName();

            HtmlToPdfDocument document = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Out = path,
                    ColorMode = DinkToPdf.ColorMode.Grayscale
                }
            };

            foreach (HtmlDocument item in documents)
            {
                document.Objects.Add(new ObjectSettings { HtmlContent = item.DocumentNode.OuterHtml });
            }

            this.converter.Convert(document);

            return path;
        }
    }
}
