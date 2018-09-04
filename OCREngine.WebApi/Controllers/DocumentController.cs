using Microsoft.AspNetCore.Mvc;

namespace OCREngine.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DocumentController : Controller
    {
        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
    }
}