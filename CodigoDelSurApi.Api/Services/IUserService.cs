using CodigoDelSurApi.Api.Models;

namespace CodigoDelSurApi.Api.Services;

public interface IUserService
{
    Task AddUserAsync(string username, string password);
    Task<bool> Exists(string username);
    Task<UserPreferencesModel> GetPreferences(int userId);
    Task SavePreferences(int userId, UserPreferencesModel preferences);
}