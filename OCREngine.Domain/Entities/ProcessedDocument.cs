using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;

namespace OCREngine.Domain.Entities
{
    public class ProcessedDocument : TableEntity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessedDocument"/> class.
        /// </summary>
        /// <param name="rowKey">The row key.</param>
        /// <param name="partitionKey">The partition  key.</param>
        public ProcessedDocument(string rowKey, string partitionKey)
        {
            this.PartitionKey = partitionKey;
            this.RowKey = rowKey;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProcessedDocument"/> class.
        /// </summary>
        public ProcessedDocument()
        {
        }

        /// <summary>
        /// Gets or sets the download URL.
        /// </summary>
        /// <value>
        /// The download URL of the Processed Document.
        /// </value>
        public string DownloadUrl { get; set; }
    }
}
