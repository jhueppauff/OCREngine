using Microsoft.AspNetCore.Mvc;
using OCREngine.WebApi.Models;
using System;
using System.Threading.Tasks;

namespace OCREngine.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : Controller
    {
        // POST api/values
        [HttpPost]
        public async Task ProcessDocument([FromBody] Models.DocumentInput document)
        {
            if (document != null)
            {

            }
        }

        [HttpPost]
        public async Task<Guid> QueueDocument([FromBody] Models.DocumentInput document)
        {
            if (document != null)
            {

                return Guid.NewGuid();
            }

            return Guid.Empty;
        }

        [HttpGet("Id")]
        public async Task<DocumentProcessingStatus> GetProcessingStatus([FromHeader] string Id)
        {
            if (Id != null)
            {
                Guid.TryParse(Id, out Guid JobId);

                DocumentProcessingStatus processingStatus = new DocumentProcessingStatus
                {
                    JobId = JobId
                };

                return processingStatus;
            }

            return null;
        }
    }
}