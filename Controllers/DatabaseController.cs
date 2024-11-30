using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MigrationAPI.Controllers
{
    [ApiController]
    [Route("api/database")]
    public class DatabaseController : ControllerBase
    {
        private readonly MigrationDbContext _context;

        public DatabaseController(MigrationDbContext context)
        {
            _context = context;
        }

        [HttpGet("test-connection")]
        public async Task<IActionResult> TestDatabaseConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();

                if (canConnect)
                {
                    return Ok(new { Message = "Database connection successful!" });
                }

                // Intentar abrir la conexión para obtener más detalles del error
                try
                {
                    await _context.Database.OpenConnectionAsync();
                    return Ok(new { Message = "Database connection successful!" });
                }
                catch (Exception ex)
                {
                    // Capturar el error específico de la conexión
                    return StatusCode(500, new { Message = "Failed to connect to the database.", Details = ex.Message });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "General error during database connection check.", Details = ex.Message });
            }
        }
    }

}
