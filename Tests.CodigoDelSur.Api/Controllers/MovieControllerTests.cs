using System.Security.Claims;
using CodigoDelSurApi.Api.Controllers;
using CodigoDelSurApi.Api.Models;
using CodigoDelSurApi.Api.Services;
using CodigoDelSurApi.Infrastructure.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.CodigoDelSur.Api.Controllers;

[TestFixture]
public class MovieControllerTests
{
    private Mock<ITMDbService> _mockTMDbService;
    private Mock<IUserService> _mockUserService;
    private MovieController _controller;

    [SetUp]
    public void Setup()
    {
        _mockTMDbService = new Mock<ITMDbService>();
        _mockUserService = new Mock<IUserService>();
        _controller = new MovieController(_mockTMDbService.Object, _mockUserService.Object);

        // Mock HttpContext for User.Identity
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
        }));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Test]
    public async Task SearchMovies_ReturnsOk_WhenPreferencesAreSet()
    {
        // Arrange
        var preferences = new UserPreferencesModel { Language = LanguageEnum.English, Genre = GenreEnum.Action };
        _mockUserService.Setup(s => s.GetPreferences(It.IsAny<int>())).ReturnsAsync(preferences);
        _mockTMDbService.Setup(s => s.DiscoverMoviesAsync(It.IsAny<string>(), It.IsAny<GenreEnum>())).ReturnsAsync(new List<MovieModel>());

        // Act
        var result = await _controller.SearchMovies();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _mockTMDbService.Verify(s => s.DiscoverMoviesAsync("en-US", GenreEnum.Action), Times.Once);
    }

    [Test]
    public async Task SearchMovies_ReturnsUnauthorized_WhenUserIdIsNotAvailable()
    {
        // Arrange
        _controller.ControllerContext.HttpContext = new DefaultHttpContext(); // No user

        // Act
        var result = await _controller.SearchMovies();

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }
}