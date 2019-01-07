namespace OCREngine.Function.Entities
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;

    public class OcrRequest : TableEntity
    {
        public OcrRequest(string partitionKey, string rowKey) : base(partitionKey, rowKey)
        {
        }

        public OcrRequest()
        {

        }

        public string BlobUri { get; set; }

        public string ProcessingState { get; set; }

        public string ExceptionMessage { get; set; }

        public string DownloadUrl { get; set; }
    }
}