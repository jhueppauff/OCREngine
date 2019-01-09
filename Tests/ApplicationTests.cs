using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCREngine.Application.Storage;
using System;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ApplicationTests
    {
        private QueueClient client;
        private IConfiguration configuration;

        [TestInitialize]
        public async Task Initialize()
        {
            configuration = GetConfiguration();
            client = new QueueClient(configuration.GetSection("AzureStorage:StorageConnectionString").Value);

            await client.PrepareEnviroment();
        }


        [TestMethod]
        public async Task QueueItem()
        {
            await client.QueueDocumentAsync(new OCREngine.Domain.Entities.OcrRequest() { PartitionKey = Guid.NewGuid().ToString(), RowKey = Guid.NewGuid().ToString() }).ConfigureAwait(false);
        }

        [TestMethod]
        public async Task GetNextRequestAsync_Should_Return_A_Result()
        {
            await client.QueueDocumentAsync(new OCREngine.Domain.Entities.OcrRequest() { PartitionKey = Guid.NewGuid().ToString(), RowKey = Guid.NewGuid().ToString() }).ConfigureAwait(false);

            string message = await client.GetNextRequestAsync();

            message.Should().NotBeNullOrWhiteSpace();
        }

        [TestMethod]
        public async Task GetNextRequestAsync_Should_Dequeue_The_Message_After_Read()
        {
            await client.QueueDocumentAsync(new OCREngine.Domain.Entities.OcrRequest() { PartitionKey = Guid.NewGuid().ToString(), RowKey = Guid.NewGuid().ToString() }).ConfigureAwait(false);
            await client.QueueDocumentAsync(new OCREngine.Domain.Entities.OcrRequest() { PartitionKey = Guid.NewGuid().ToString(), RowKey = Guid.NewGuid().ToString() }).ConfigureAwait(false);

            string message1 = await client.GetNextRequestAsync().ConfigureAwait(false);
            string message2 = await client.GetNextRequestAsync().ConfigureAwait(false);

            message1.Should().NotBeSameAs(message2);
        }

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("testconfig.json", false).Build();
        }
    }
}
