using CodigoDelSurApi.Api.Models;
using CodigoDelSurApi.Infrastructure.Enums;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;

namespace CodigoDelSurApi.Api.Services;

public partial class TMDbService : ITMDbService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private const string BaseUrl = "https://api.themoviedb.org/3/";
    private string GetApiKey() => _configuration["TMDb:API_KEY"];


    public TMDbService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(BaseUrl);
        _configuration = configuration;
    }

    public async Task<IEnumerable<MovieModel>> DiscoverMoviesAsync(string language = null,  GenreEnum? genre = null)
    {
        QueryBuilder qb = new QueryBuilder();
        qb.Add("api_key", GetApiKey());
        if (!string.IsNullOrWhiteSpace(language))
            qb.Add("language", language);
        if (genre.HasValue)
            qb.Add("with_genres", ((int)genre.Value).ToString());

        var uri = $"discover/movie{qb}";
        var response = await _httpClient.GetAsync(uri);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<MovieSearchResultModel>(content);
        return result.Results;
    }

}