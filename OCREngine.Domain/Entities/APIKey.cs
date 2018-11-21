using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCREngine.Domain.Entities
{
    public class APIKey : ITableEntity
    {
        public Guid Id { get; set; }

        public Guid Key { get; set; }

        public DateTime CreatedOn { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }

        public string ETag { get; set; }
    }
}
