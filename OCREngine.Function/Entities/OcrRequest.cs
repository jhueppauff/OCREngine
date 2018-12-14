namespace OCREngine.Function.Entities
{
    using Microsoft.WindowsAzure.Storage.Table;
    using System;

    public class OcrRequest : TableEntity
    {
        public Guid RequestId { get => Guid.Parse(RowKey); set => RowKey = value.ToString(); }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public Uri BlobUri { get; set; }

        public ProcessingStates ProcessingState { get; set; }

        public AggregateException ProcessingException { get; set; }

        public string ExceptionMessage { get; set; }

        public new string PartitionKey { get; set; }

        public new string RowKey { get; set; }

        public string DownloadUrl { get; set; }

        public static explicit operator OcrRequest(string value)
        {
            return new OcrRequest();
        }
    }
}
