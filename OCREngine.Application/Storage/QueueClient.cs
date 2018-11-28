namespace OCREngine.Application.Storage
{
    using AzureStorageAdapter.Queue;
    using System.Threading.Tasks;

    public class QueueClient
    {
        private readonly QueueStorageAdapter queue;

        private const string name = "ocrprocessing";

        public QueueClient(string queueConnectionString)
        {
            this.queue = new QueueStorageAdapter(queueConnectionString); 
        }

        public async Task PrepareEnviroment()
        {
            await this.queue.CreateQueueAsync(name).ConfigureAwait(false);
        }

        public async Task QueueDocumentAsync(Domain.Entities.OcrRequest request)
        {
            await this.queue.AddEntryToQueueAsync(name, $"{request.PartitionKey};{request.RowKey}").ConfigureAwait(false);
        }

        public async Task<string> GetNextRequestAsync()
        {
            string result = await this.queue.PeekNextMessageStringAsync(name).ConfigureAwait(false);
            await this.queue.RemoveNextMessageAsync(name);

            return result;
        }
    }
}