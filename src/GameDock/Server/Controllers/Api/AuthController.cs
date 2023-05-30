using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using GameDock.Server.Infrastructure.Entities.Identity;
using GameDock.Server.Utils;
using GameDock.Shared.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GameDock.Server.Controllers.Api;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AuthOptions _options;
    private readonly UserManager<AppUser> _userManager;

    public AuthController(AuthOptions options, UserManager<AppUser> userManager)
    {
        _options = options;
        _userManager = userManager;
    }

    [HttpPost("token")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Token([FromBody]LoginRequest request)
    {
        var identity = await GetIdentity(request.Login, request.Password);
        if (identity == null)
        {
            return BadRequest("Invalid username or password." );
        }

        var jwt = GenerateJwtToken(identity);

        return Ok(jwt);
    }

    private async Task<ClaimsIdentity> GetIdentity(string username, string password)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user is null || !await _userManager.CheckPasswordAsync(user, password))
        {
            return null;
        }

        var claims = new List<Claim> { new(ClaimsIdentity.DefaultNameClaimType, user.UserName!), };

        var userRoles = await _userManager.GetRolesAsync(user);
        claims.AddRange(userRoles.Select(x => new Claim(ClaimsIdentity.DefaultRoleClaimType, x)));

        var claimsIdentity = new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType);

        return claimsIdentity;
    }

    private string GenerateJwtToken(ClaimsIdentity identity)
    {
        var now = DateTime.UtcNow;
        var cred = new SigningCredentials(_options.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(_options.Lifetime),
            signingCredentials: cred);

        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        return encodedJwt;
    }
}