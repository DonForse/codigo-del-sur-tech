using CodigoDelSurApi.Api.Models;
using CodigoDelSurApi.Infrastructure;
using CodigoDelSurApi.Infrastructure.DataEntities;
using CodigoDelSurApi.Infrastructure.Enums;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;

namespace CodigoDelSurApi.Api.Services;

public class UserService : IUserService
{
    private readonly ICodigoDelSurDbContext _context;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(ICodigoDelSurDbContext context, IPasswordHasher passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task AddUserAsync(string username, string password)
    {
        var hashedPassword = _passwordHasher.HashPassword(password);
        var user = new User() { Username = username, PasswordHash = hashedPassword };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> Exists(string username)
    {
        return await _context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task<UserPreferencesModel> GetPreferences(int userId)
    {
        var preferences = await _context.UserPreferences.FirstOrDefaultAsync(x=>x.UserId == userId);
        if (preferences == null) return null;

        //this conversion could be done with automapper.
        return MapUserPreferencesModel(preferences);
    }



    public async Task SavePreferences(int userId, UserPreferencesModel preferencesModel)
    {
        var existingPreferences = await _context.UserPreferences.FindAsync(userId);
        if (existingPreferences != null)
        {
            UserPreferences preferences = MapUserPreferences(userId, preferencesModel);
            existingPreferences.Genre = preferences.Genre;
            existingPreferences.Language = preferences.Language;
            _context.UserPreferences.Update(existingPreferences);
        }
        else
        {
            UserPreferences preferences = MapUserPreferences(userId, preferencesModel);
            _context.UserPreferences.Add(preferences);
        }
        await _context.SaveChangesAsync();
    }

    #region Mapping
    //these conversions could be done with automapper.

    private static UserPreferencesModel MapUserPreferencesModel(UserPreferences? preferences) => 
        new UserPreferencesModel
        {
            Genre = (GenreEnum)preferences.Genre,
            Language =EnumHelper.ParseLanguage( preferences.Language),
        };

    private static UserPreferences MapUserPreferences(int userId, UserPreferencesModel preferencesModel) =>
        new UserPreferences
        {
            UserId = userId,
            Genre = (int)preferencesModel.Genre,
            Language = preferencesModel.Language.GetDescription()
        };
    #endregion
}