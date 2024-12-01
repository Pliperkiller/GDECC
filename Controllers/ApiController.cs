using Microsoft.AspNetCore.Mvc;

namespace MigrationAPI.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : Controller
    {
        [HttpGet("status")]
        public IActionResult GetStatus()
        {
            return Ok(new { Message = "Migration API is running" });
        }
    }
}
