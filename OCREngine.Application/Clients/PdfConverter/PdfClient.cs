using DinkToPdf;
using DinkToPdf.Contracts;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;

namespace OCREngine.Application.Clients.Output.PdfConverter
{
    public class PdfClient
    {
        private readonly IConverter converter;

        public PdfClient(IConverter converter)
        {
            this.converter = converter;
        }

        public string ConvertHtmlToPdf(List<HtmlDocument> documents)
        {
            string path = $"{Path.GetTempPath()}\\{Guid.NewGuid().ToString()}.pdf";

            HtmlToPdfDocument document = new HtmlToPdfDocument()
            {
                GlobalSettings =
                {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait,
                    Out = path,
                    ColorMode = ColorMode.Grayscale
                }
            };

            foreach (HtmlDocument item in documents)
            {
                document.Objects.Add(new ObjectSettings { HtmlContent = item.DocumentNode.OuterHtml });
            }


            converter.Convert(document);

            return path;
        }
    }
}
