using CodigoDelSurApi.Infrastructure;
using CodigoDelSurApi.Infrastructure.DataEntities;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodigoDelSurApi.Api.Services;

public class AuthService : IAuthService
{
    private readonly ICodigoDelSurDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IPasswordHasher _passwordHasher;

    public AuthService(ICodigoDelSurDbContext context, IConfiguration configuration, IPasswordHasher passwordHasher)
    {
        _context = context;
        _configuration = configuration;
        _passwordHasher = passwordHasher;
    }

    public async Task<User> AuthenticateUser(string username, string password)
    {
        // Check if user exists
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
        {
            return null; // User not found
        }

        // Verify password
        var passwordMatches = _passwordHasher.VerifyHashedPassword(user.PasswordHash, password);
        if (passwordMatches == PasswordVerificationResult.Failed)
        {
            return null; // Password does not match
        }

        return user;
    }
}