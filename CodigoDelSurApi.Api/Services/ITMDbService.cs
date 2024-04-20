using CodigoDelSurApi.Api.Models;
using CodigoDelSurApi.Infrastructure.Enums;

namespace CodigoDelSurApi.Api.Services;

public interface ITMDbService
{
    Task<IEnumerable<MovieModel>> DiscoverMoviesAsync(string language = null, GenreEnum? genre = null);
}