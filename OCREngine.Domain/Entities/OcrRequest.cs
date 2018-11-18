namespace OCREngine.Domain.Entities
{
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Table;
    using OCREngine.Domain.Infrastructure;
    using System;
    using System.Collections.Generic;

    public class OcrRequest : ValueObject, ITableEntity
    {
        public Guid RequestId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime FinishDate { get; set; }

        public Uri BlobUri { get; set; }

        public ProcessingStates ProcessingState { get; set; }

        public AggregateException ProcessingException { get; set; }

        public string ExceptionMessage { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string ETag { get; set; }

        private OcrRequest()
        {
                
        }

        public static explicit operator OcrRequest(string value)
        {
            return new OcrRequest();
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            throw new NotImplementedException();
        }

        public void ReadEntity(IDictionary<string, EntityProperty> properties, OperationContext operationContext)
        {
            throw new NotImplementedException();
        }

        public IDictionary<string, EntityProperty> WriteEntity(OperationContext operationContext)
        {
            throw new NotImplementedException();
        }
    }
}
