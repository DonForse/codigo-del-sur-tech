using System.Security.Claims;
using CodigoDelSurApi.Api.Models;
using CodigoDelSurApi.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodigoDelSurApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController( IUserService userService)
    {
        _userService = userService;
    }


    [Authorize]
    [HttpPost("preferences")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SavePreferences([FromBody] UserPreferencesModel preferences)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        

        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized(ModelState);           
        

        await _userService.SavePreferences(userId, preferences);
        return Ok("Preferences saved successfully.");

    }

    [Authorize]
    [HttpGet("preferences")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetPreferences()
    {
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized(ModelState);

        var preferences = await _userService.GetPreferences(userId);
        if (preferences == null)
        {
            return NotFound("Preferences not found.");
        }
        return Ok(preferences);
    }
}