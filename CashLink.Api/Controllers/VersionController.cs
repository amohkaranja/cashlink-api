using Microsoft.AspNetCore.Mvc;

namespace CashLink.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VersionController : ControllerBase
{
    [HttpGet]
    public IActionResult GetVersion()
    {
        return Ok(new
        {
            Version = "1.0.0",
            Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
            Message = "CashLink API - CI/CD Pipeline Test",
            Timestamp = DateTime.UtcNow,
            Hostname = Environment.MachineName
        });
    }
}
