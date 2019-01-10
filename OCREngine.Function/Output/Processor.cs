namespace OCREngine.Function.Output
{
    using System.Collections.Generic;
    using OCREngine.Domain.Entities.Vision;
    using HtmlAgilityPack;
    using DinkToPdf;
    using OCREngine.Function.Output.PdfConverter;

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

            WkHtmlToPdf.Preload();
            var converter = new SynchronizedConverter(new PdfTools());

            PdfClient pdfClient = new PdfClient(converter);

            return pdfClient.ConvertHtmlToPdf(htmlDocuments);
        }
    }
}