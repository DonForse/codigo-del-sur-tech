using System.Security.Claims;
using CodigoDelSurApi.Api.Services;
using CodigoDelSurApi.Infrastructure.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CodigoDelSurApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MovieController : ControllerBase
{
    private readonly ITMDbService _tmDbService;
    private readonly IUserService _userService;

    public MovieController(ITMDbService tmDbService, IUserService userService)
    {
        _tmDbService = tmDbService;
        _userService = userService;
    }

    [Authorize]
    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchMovies()
    {        
        if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
            return Unauthorized(ModelState);
        
        var preferences = await _userService.GetPreferences(userId);

        var filteredMovies = await _tmDbService.DiscoverMoviesAsync(preferences?.Language.GetDescription(), preferences?.Genre);
        return Ok(filteredMovies);
    }
}