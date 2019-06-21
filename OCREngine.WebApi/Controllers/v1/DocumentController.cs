using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using OCREngine.WebApi.Vision;
using OCREngine.WebApi.Vision.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OCREngine.WebApi.Document;
using HtmlAgilityPack;
using System.Net.Http;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using OCREngine.Domain.Entities.Vision;
using System.Net;
using OCREngine.Application.Storage;
using OCREngine.Application.Document;
using OCREngine.Domain.Entities;

namespace OCREngine.WebApi.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Microsoft.AspNetCore.Mvc.Controller" />
    [Route("api/v1/[controller]")]
    [ApiController]
    [RequireHttps]
    public class DocumentController : Controller
    {
        /// <summary>
        /// The configuration
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        /// The converter
        /// </summary>
        private readonly IConverter converter;

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentController"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="converter">The converter.</param>
        public DocumentController(IConfiguration configuration, IConverter converter)
        {
            this.configuration = configuration;
            this.converter = converter;
        }

        /// <summary>
        /// Pings this instance.
        /// </summary>
        /// <returns>Returns <see cref="IActionResult"/></returns>
        [HttpGet("Ping")]
        public IActionResult Ping()
        {
            return this.Ok(true);
        }

        /// <summary>
        /// Gets this instance.
        /// </summary>
        /// <returns>Returns <see cref="IActionResult"/> HTTP Status 405</returns>
        [HttpGet]
        public IActionResult Get()
        {
            return this.StatusCode(405);
        }

        /// <summary>
        /// Post Processing of the document call.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="apiKey">The API key.</param>
        /// <returns>Returns <see cref="Task{ActionResult}"/></returns>
        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Post([FromBody] Domain.Infrastructure.Document document, [FromHeader] string apiKey)
        {
            if (document != null && document.DownloadUrl != null)
            {
                if (string.IsNullOrEmpty(document.Name))
                {
                    document.Name = DateTime.Now.ToString() + ".pdf";
                }

                string filePath = Path.GetTempFileName();

                using (WebClient client = new WebClient())
                {
                    await client.DownloadFileTaskAsync(new Uri(document.DownloadUrl), filePath).ConfigureAwait(false);
                }

                Processor processor = new Processor(this.configuration, this.converter);

                var result = await processor.ProcessDocument(filePath).ConfigureAwait(false);

                Operation operation = new Operation
                {
                    FileId = Guid.NewGuid().ToString(),
                    OperationId = Guid.NewGuid().ToString(),
                };

                operation.FileUrl = $"{this.Request.Scheme}://{this.Request.Host}/api/document/{operation.OperationId}/{operation.FileId}";

                await this.CreateTableEntry(operation, result).ConfigureAwait(false);

                return this.Ok(operation);
            }
            else
            {
                return this.BadRequest();
            }
        }

        /// <summary>
        /// Deletes the specified operation identifier.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <returns>Returns <see cref="Task{IActionResult}"/></returns>
        [Authorize]
        [HttpDelete]
        [Route("{OperationId}/{FileId}")]
        public async Task<IActionResult> Delete(string operationId, string fileId)
        {
            TableStore tableStore = new TableStore(this.configuration.GetSection("AzureBlobStorageSettings:ConnectionString").Value);

            var entry = await tableStore.GetEntry(fileId, operationId).ConfigureAwait(false);

            FileStore fileStore = new FileStore(this.configuration.GetSection("AzureBlobStorageSettings:ConnectionString").Value, this.configuration.GetSection("AzureBlobStorageSettings:ContainerName").Value);

            try
            {
                await fileStore.DeleteBlob(entry.DownloadUrl).ConfigureAwait(false);
                await tableStore.DeleteEntry(fileId, operationId).ConfigureAwait(false);
                return this.Ok();
            }
            catch (Exception)
            {
                return this.NotFound();
            }
        }

        /// <summary>
        /// Gets the processed file.
        /// </summary>
        /// <param name="operationId">The operation identifier.</param>
        /// <param name="fileId">The file identifier.</param>
        /// <returns>Returns <see cref="Task{IActionResult}"/></returns>
        [Authorize]
        [HttpGet]
        [Route("{OperationId}/{FileId}")]
        public async Task<IActionResult> GetFile(string operationId, string fileId)
        {
            TableStore tableStore = new TableStore(this.configuration.GetSection("AzureBlobStorageSettings:ConnectionString").Value);
            return this.Ok(await tableStore.GetEntry(fileId, operationId).ConfigureAwait(false));
        }

        /// <summary>
        /// Creates the table entry.
        /// </summary>
        /// <param name="operation">The operation.</param>
        /// <param name="url">The URL.</param>
        /// <returns>Returns <see cref="Task"/></returns>
        private async Task CreateTableEntry(Domain.Entities.Operation operation, string url)
        {
            TableStore tableStore = new TableStore(this.configuration.GetSection("AzureBlobStorageSettings:ConnectionString").Value);

            ProcessedDocument processedDocument = new ProcessedDocument(operation.FileId, operation.OperationId)
            {
                DownloadUrl = url
            };

            await tableStore.AddEntry(processedDocument).ConfigureAwait(false);
        }
    }
}