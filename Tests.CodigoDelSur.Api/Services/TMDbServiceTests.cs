using System.Net;
using CodigoDelSurApi.Api.Models;
using CodigoDelSurApi.Api.Services;
using CodigoDelSurApi.Infrastructure.Enums;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;

namespace Tests.CodigoDelSur.Api.Services;

[TestFixture]
public class TMDbServiceTests
{
    private Mock<HttpClientHandler> _handlerMock;
    private HttpClient _httpClient;
    private TMDbService _tmDbService;
    private Mock<IConfiguration> _mockConfiguration;

    [SetUp]
    public void Setup()
    {
        _handlerMock = new Mock<HttpClientHandler>();
        _httpClient = new HttpClient(_handlerMock.Object)
        {
            BaseAddress = new Uri("https://api.themoviedb.org/3/")
        };
        _mockConfiguration = new Mock<IConfiguration>();
        _mockConfiguration.Setup(c => c["TMDb:API_KEY"]).Returns("Your_API_Key_Here");

        _tmDbService = new TMDbService(_httpClient, _mockConfiguration.Object);
    }
    [TearDown]
    public void TearDown() {
        _httpClient?.Dispose();
    }

    [Test]
    public async Task DiscoverMoviesAsync_ReturnsMovies_WhenCalledWithValidParameters()
    {
        // Arrange
        var fakeResponse = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK,
            Content = new StringContent(JsonConvert.SerializeObject(new MovieSearchResultModel
            {
                Results = new List<MovieModel>
                {
                    new MovieModel { Title = "Test Movie", Id = 1 }
                }
            }))
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.Is<HttpRequestMessage>(req =>
                    req.Method == HttpMethod.Get
                    && req.RequestUri.ToString().Contains("discover/movie")),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(fakeResponse);

        // Act
        var movies = await _tmDbService.DiscoverMoviesAsync("en-US", GenreEnum.Action);

        // Assert
        Assert.That(movies, Is.Not.Empty);
        Assert.That(movies.First().Title, Is.EqualTo("Test Movie"));
    }
}