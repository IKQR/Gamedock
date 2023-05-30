using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api;

[ApiController]
[Route("/api/test")]
public class TestController : ControllerBase
{
    [HttpGet]
    [Authorize]
    public IActionResult Index()
    {
        if (HttpContext.User.Identity is not null) return Ok(HttpContext.User.Identity.Name);
        return BadRequest();
    }
}