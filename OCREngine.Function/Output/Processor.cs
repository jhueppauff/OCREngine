namespace OCREngine.Function.Output
{
    using System.Collections.Generic;
    using OCREngine.Domain.Entities.Vision;
    using HtmlAgilityPack;
    using DinkToPdf;
    using OCREngine.Function.Output.PdfConverter;
    using OcrEngine.Function.Output;
    using System.Threading;

    public class Processor
    {
        public string BuildDocumentFromOcrResult(List<string> files, List<OcrResults> ocrResults, Microsoft.Azure.WebJobs.ExecutionContext context)
        {
            HtmlParser htmlParser = new HtmlParser();
            List<HtmlDocument> htmlDocuments = new List<HtmlDocument>();
            int i = 0;
            foreach (var ocrResult in ocrResults)
            {
                htmlDocuments.Add(htmlParser.CreateHtmlFromVisionResult(ocrResult, files[i]));
                i++;
            }

            AssemblyLoader.Preload(context, "libwkhtmltox");
            var converter = new SynchronizedConverter(new PdfTools());

            PdfClient pdfClient = new PdfClient(converter);

            return pdfClient.ConvertHtmlToPdf(htmlDocuments);
        }
    }
}