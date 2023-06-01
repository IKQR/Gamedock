using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api;

[Authorize]
[ApiController]
[Route("/api/fleet/info")]
public class FleetInfoController : ControllerBase
{
    
}