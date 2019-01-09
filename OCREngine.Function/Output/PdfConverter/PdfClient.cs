using DinkToPdf;
using DinkToPdf.Contracts;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;

namespace OCREngine.Function.Output.PdfConverter
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
            try
            {
                string path = Path.GetTempFileName();                

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
            catch (System.Exception ex)
            {
                
                throw;
            }
        }
    }
}
