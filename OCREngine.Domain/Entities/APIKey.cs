using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;

namespace OCREngine.Domain.Entities
{
    public class ApiKey : ITableEntity
    {
        public Guid Id { get; set; }

        public Guid Key { get; set; }

        public DateTime CreatedOn { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string ETag { get; set; }

        public DateTimeOffset Timestamp { get; set; }

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
