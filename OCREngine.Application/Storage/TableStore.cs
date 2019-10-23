using System.Threading.Tasks;
using OCREngine.Domain.Entities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace OCREngine.Application.Storage
{
    /// <summary>
    /// Class to deal with Azure Table Storage
    /// </summary>
    public class TableStore
    {
        /// <summary>
        /// Azure Cloud Table Client
        /// </summary>
        private readonly CloudTable cloudTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableStore"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string to the Storage Account.</param>
        public TableStore(string connectionString)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            this.cloudTable = tableClient.GetTableReference("documents");
        }

        /// <summary>
        /// Adds the entry.
        /// </summary>
        /// <param name="processedDocument">The processed document.</param>
        /// <returns>Returns <see cref="Task{VoidResult}"/></returns>
        public async Task AddEntry(ProcessedDocument processedDocument)
        {
            await this.cloudTable.CreateIfNotExistsAsync();

            TableOperation insertOperation = TableOperation.Insert(processedDocument);

            await this.cloudTable.ExecuteAsync(insertOperation);
        }

        /// <summary>
        /// Gets the entry.
        /// </summary>
        /// <param name="rowKey">The row key.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <returns>Returns <see cref="Task{ProcessedDocument}"/></returns>
        public async Task<ProcessedDocument> GetEntry(string rowKey, string partitionKey)
        {
            TableOperation retrieveOperation = TableOperation.Retrieve<ProcessedDocument>(partitionKey, rowKey);

            TableResult retrievedResult = await this.cloudTable.ExecuteAsync(retrieveOperation);

            if (retrievedResult.Result != null)
            {
                return (ProcessedDocument)retrievedResult.Result;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Deletes the entry.
        /// </summary>
        /// <param name="rowKey">The row key.</param>
        /// <param name="partitionKey">The partition key.</param>
        /// <returns>Returns <see cref="Task{VoidResult}"/></returns>
        public async Task DeleteEntry(string rowKey, string partitionKey)
        {
            // Assign the result to a CustomerEntity.
            ProcessedDocument deleteEntity = await this.GetEntry(rowKey, partitionKey);

            // Create the Delete TableOperation.
            if (deleteEntity != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(deleteEntity);

                // Execute the operation.
                await this.cloudTable.ExecuteAsync(deleteOperation);
            }
        }
    }
}
