namespace OCREngine.Function.Output
{
    using System.Collections.Generic;
    using OCREngine.Domain.Entities.Vision;
    using HtmlAgilityPack;
    using DinkToPdf;

    public class Processor
    {
        public string BuildDocumentFromOcrResult(List<string> file, List<OcrResults> ocrResults)
        {
            HtmlParser htmlParser = new HtmlParser();
            List<HtmlDocument> htmlDocuments = new List<HtmlDocument>();

            foreach (var ocrResult in ocrResults)
            {
                htmlDocuments.Add(htmlParser.CreateHtmlFromVisionResult(ocrResult));
            }

            var converter = new BasicConverter(new PdfTools());

            PdfConverter.PdfClient pdfClient = new PdfConverter.PdfClient(converter);

            return pdfClient.ConvertHtmlToPdf(htmlDocuments);
        }
    }
}