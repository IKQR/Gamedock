using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api;

[ApiController]
[Route("test")]
public class TestController : ControllerBase
{
    [Authorize]
    [HttpGet("/api/test")]
    public IActionResult Index()
    {
        if (HttpContext.User.Identity is not null) return Ok(HttpContext.User.Identity.Name);
        return BadRequest();
    }
}