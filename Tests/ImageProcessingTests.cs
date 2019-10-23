using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ImageProcessingTests
    {
        private AzureStorageAdapter.Queue.QueueStorageAdapter queueClient;
        private IConfiguration configuration;
        private AzureStorageAdapter.Table.TableStorageAdapter tableClient;

        [TestInitialize]
        public async Task Initialize()
        {
            configuration = GetConfiguration();
            queueClient = new AzureStorageAdapter.Queue.QueueStorageAdapter(configuration.GetSection("AzureStorage:StorageConnectionString").Value);

            await queueClient.CreateQueueAsync("ImageQueue");

            tableClient = new AzureStorageAdapter.Table.TableStorageAdapter(configuration.GetSection("AzureStorage:StorageConnectionString").Value);

            if (!await tableClient.TableExits("ImageRequests").ConfigureAwait(false))
            {
                await tableClient.CreateNewTable("ImageRequests").ConfigureAwait(false);
            }
        }

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("testconfig.json", false).Build();
        }
    }
}
