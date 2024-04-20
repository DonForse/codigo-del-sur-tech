using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CodigoDelSurApi.Api.Models;
using CodigoDelSurApi.Api.Services;
using CodigoDelSurApi.Infrastructure.DataEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace CodigoDelSurApi.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;

    public AuthController(IAuthService authService, IConfiguration configuration, IUserService userService)
    {
        _authService = authService;
        _configuration = configuration;
        _userService = userService;
    }

    [HttpPost("register")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> Register([FromBody] UserModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if the username already exists
        if (await _userService.Exists(model.Username))
        {
            return BadRequest("Username is already taken.");
        }

        try
        {
            // Add the user to the database
            await _userService.AddUserAsync(model.Username, model.Password);
            return Ok("User registered successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while registering the user: " + ex.Message);
        }
    }


    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] UserModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _authService.AuthenticateUser(model.Username, model.Password);
        if (user == null)
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = GenerateJwtToken(user);
        return Ok(new { Token = token, Username = user.Username });
    }

    private string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            }),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public static string GenerateSecureKey()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[32]; // 256 bits for HMAC-SHA256
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes); // Convert to Base64 for easier handling and storage
        }
    }
}