using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameDock.Server.Infrastructure.Entities.Identity;
using GameDock.Shared.Dto;
using GameDock.Shared.Requests;
using GameDock.Shared.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GameDock.Server.Controllers.Api.Admin;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/user")]
public class UserManagementController : ControllerBase
{
    private readonly UserManager<AppUser> _userManager;

    public UserManagementController(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AppUserDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userManager.GetUsersInRoleAsync("User");
        var admins = await _userManager.GetUsersInRoleAsync("Admin");

        var result =
            admins.Select(x => new AppUserDto { Id = x.Id, Login = x.UserName, IsAdmin = true }).UnionBy(
                users.Select(x => new AppUserDto { Id = x.Id, Login = x.UserName, IsAdmin = false }),
                u => u.Id
            );

        return Ok(result.ToList());
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateUserResponse), 200)]
    [ProducesResponseType(typeof(string), 400)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.Login);

        if (user is not null)
        {
            return BadRequest("User with requested login already exists");
        }

        user = new AppUser(request.Login);

        var password = PasswordGenerator.GenerateStrongPassword();

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return BadRequest(string.Join(" | ", result.Errors.Select(x => $"{x.Code} : {x.Description}")));
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            return BadRequest(string.Join(" | ", result.Errors.Select(x => $"{x.Code} : {x.Description}")));
        }

        return Ok(new CreateUserResponse
        {
            Id = user.Id,
            Login = user.UserName,
            Password = password,
            Roles = new[] { "User" },
        });
    }

    [HttpGet("{userId:required}")]
    [ProducesResponseType(typeof(AppUserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Get([FromRoute] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

        return Ok(new AppUserDto
        {
            Id = user.Id,
            Login = user.UserName,
            IsAdmin = isAdmin,
        });
    }

    [HttpPut("{userId:required}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update([FromRoute] string userId, [FromBody] UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        if (HttpContext.User.Identity?.Name == user.UserName)
        {
            return BadRequest();
        }

        var roles = await _userManager.GetRolesAsync(user);

        if (roles.Any(x => x == "Admin") && !request.IsAdmin)
        {
            await _userManager.AddToRoleAsync(user, "User");
            await _userManager.RemoveFromRoleAsync(user, "Admin");
        }
        else if (roles.All(x => x != "Admin") && request.IsAdmin)
        {
            await _userManager.AddToRoleAsync(user, "Admin");
            await _userManager.RemoveFromRoleAsync(user, "User");
        }

        return Ok();
    }

    [HttpDelete("{userId:required}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete([FromRoute] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        if (HttpContext.User.Identity?.Name == user.UserName)
        {
            return BadRequest();
        }

        await _userManager.DeleteAsync(user);

        return Ok();
    }

    [HttpPost("{userId:required}/reset")]
    [ProducesResponseType(typeof(CreateUserResponse), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> ResetPassword([FromRoute] string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return NotFound();
        }

        if (HttpContext.User.Identity?.Name == user.UserName)
        {
            return BadRequest();
        }

        var password = PasswordGenerator.GenerateStrongPassword();

        var result = await _userManager.RemovePasswordAsync(user);

        if (!result.Succeeded)
        {
            return BadRequest(string.Join(" | ", result.Errors.Select(x => $"{x.Code} : {x.Description}")));
        }

        await _userManager.AddPasswordAsync(user, password);

        var roles = await _userManager.GetRolesAsync(user);

        return Ok(new CreateUserResponse
        {
            Id = user.Id,
            Login = user.UserName,
            Password = password,
            Roles = roles.ToArray(),
        });
    }
}