using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DinkToPdf.Contracts;
using HtmlAgilityPack;
using OCREngine.Domain.Entities.Vision;
using Microsoft.Extensions.Configuration;
using OCREngine.Application.Vision;
using OCREngine.Application.Storage;
using OCREngine.Application.Document;

namespace OCREngine.WebApi
{
    /// <summary>
    /// Document Processor Class
    /// </summary>
    public class Processor
    {
        /// <summary>
        /// The vision service client
        /// </summary>
        private readonly VisionServiceClient visionServiceClient;

        /// <summary>
        /// The file store
        /// </summary>
        private readonly FileStore fileStore;

        /// <summary>
        /// PDF converter
        /// </summary>
        private readonly IConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="Processor"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public Processor(IConfiguration configuration)
        {
            this.visionServiceClient = new VisionServiceClient(configuration.GetSection("VisionApiSubscriptionKey").Value, configuration.GetSection("VisionApiEndpoint").Value);
            this.fileStore = new FileStore(configuration.GetSection("AzureBlobStorageSettings:ConnectionString").Value, configuration.GetSection("AzureBlobStorageSettings:ContainerName").Value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Processor"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="converter">The converter.</param>
        public Processor(IConfiguration configuration, IConverter converter)
        {
            this.visionServiceClient = new VisionServiceClient(configuration.GetSection("VisionApiSubscriptionKey").Value, configuration.GetSection("VisionApiEndpoint").Value);
            this.fileStore = new FileStore(configuration.GetSection("AzureBlobStorageSettings:ConnectionString").Value, configuration.GetSection("AzureBlobStorageSettings:ContainerName").Value);
            this.converter = converter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Processor"/> class.
        /// </summary>
        /// <param name="subscriptionKey">The subscription key.</param>
        /// <param name="endpointUrl">The endpoint URL.</param>
        /// <param name="blobConnectionString">The BLOB connection string.</param>
        /// <param name="containerName">Name of the container.</param>
        public Processor(string subscriptionKey, string endpointUrl, string blobConnectionString, string containerName)
        {
            this.visionServiceClient = new VisionServiceClient(subscriptionKey, endpointUrl);
            this.fileStore = new FileStore(blobConnectionString, containerName);
        }

        /// <summary>
        /// Processes the document.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>Returns <see cref="Task{string}"/></returns>
        public async Task<string> ProcessDocument(string path)
        {
            List<OcrResults> ocrResults = new List<OcrResults>();
            PdfConverter pdfConverter = new PdfConverter();
            List<string> paths = pdfConverter.ConvertPdfToImage(path);

            foreach (var item in paths)
            {
                var result = await this.ProcessImage(item, false).ConfigureAwait(false);
                ocrResults.Add(result);
            }

            var documents = await this.BuildHtmlDocuments(ocrResults, paths).ConfigureAwait(false);

            return await this.ConvertHtmlDocumentToPdfAndUpload(documents).ConfigureAwait(false);
        }

        /// <summary>
        /// Sends a Image to the Vision API.
        /// </summary>
        /// <param name="path">Path of the Image File</param>
        /// <param name="deleteFileOnCompletion">If true, the file will be deleted on completion</param>
        /// <returns>Returns the OCR Result <see cref="OcrResults"/></returns>
        public async Task<OcrResults> ProcessImage(string path, bool deleteFileOnCompletion)
        {
            FileStream fileStream = new FileStream(path, FileMode.Open, access: FileAccess.Read);

            var result = await this.visionServiceClient.RecognizeTextAsync(fileStream).ConfigureAwait(false);

            fileStream.Close();
            fileStream.Dispose();

            if (deleteFileOnCompletion)
            {
                File.Delete(path);
            }

            return result;
        }

        /// <summary>
        /// Builds the HTML documents.
        /// </summary>
        /// <param name="ocrResults">The OCR results.</param>
        /// <param name="files">The files.</param>
        /// <returns>Returns <see cref="Task{List{HtmlDocument}}"/></returns>
        private async Task<List<HtmlDocument>> BuildHtmlDocuments(List<OcrResults> ocrResults, List<string> files)
        {
            HtmlParser htmlParser = new HtmlParser();
            List<HtmlDocument> documents = new List<HtmlDocument>();

            int page = 0;
            foreach (OcrResults result in ocrResults)
            {
                string fileUpload = await this.fileStore.UploadToBlob(await File.ReadAllBytesAsync(files[page]).ConfigureAwait(false), Guid.NewGuid() + ".jpeg", true, "image/jpeg").ConfigureAwait(false);
                documents.Add(htmlParser.CreateHtmlFromVisionResult(result, fileUpload));
                page++;
            }

            return documents;
        }

        /// <summary>
        /// Converts the HTML Document to a PDF and Uploads it to Azure Blob Storage
        /// </summary>
        /// <param name="documents">A <see cref="List{HtmlDocument}"></see> with all Pages of the OCR analyzed pdf/></param>
        /// <returns>Returns the Url of the Azure Blob with a SAS Token for 30 min</returns>
        private async Task<string> ConvertHtmlDocumentToPdfAndUpload(List<HtmlDocument> documents)
        {
            PdfConverter pdfConverter = new PdfConverter(this.converter);
            string path = pdfConverter.ConvertHtmlToPdf(documents);

            var upload = await this.fileStore.UploadToBlob(await File.ReadAllBytesAsync(path).ConfigureAwait(false), Guid.NewGuid().ToString() + ".pdf", true, "application/pdf").ConfigureAwait(false);

            return upload;
        }
    }
}
