using CodigoDelSurApi.Infrastructure.DataEntities;

namespace CodigoDelSurApi.Api.Services;

public interface IAuthService
{
    Task<User> AuthenticateUser(string username, string password);
}