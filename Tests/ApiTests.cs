using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OCREngine.WebApi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class ApiTests
    {
        private IConfiguration configuration;
        private TestServer api;

        [TestInitialize]
        public void Initialize()
        {
            configuration = GetConfiguration();

            this.api = new TestServer(WebHost
                .CreateDefaultBuilder()
                .UseStartup<Startup>());
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.api.Dispose();
        }

        [TestMethod]
        public async Task Document_Put_With_Invalid_Uri_Should_Return_An_Error()
        {
           // var response = await this.api.CreateRequest("/api/v1/document").AddHeader().SendAsync();
        }


        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("testconfig.json", false).Build();
        }
    }
}
