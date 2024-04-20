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
public class UserControllerTests
{
    private UserController _controller;
    private Mock<IUserService> _mockUserService;

    [SetUp]
    public void Setup()
    {
        _mockUserService = new Mock<IUserService>();
        _controller = new UserController(_mockUserService.Object);

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    [Test]
    public async Task SavePreferences_ReturnsOk_WhenPreferencesAreSaved()
    {
        // Arrange
        var preferences = new UserPreferencesModel { Language = LanguageEnum.English, Genre = GenreEnum.Action };

        // Act
        var result = await _controller.SavePreferences(preferences);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        _mockUserService.Verify(s => s.SavePreferences(1, preferences), Times.Once);
    }

    [Test]
    public async Task SavePreferences_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _controller.ControllerContext.HttpContext = new DefaultHttpContext(); // No user

        // Act
        var result = await _controller.SavePreferences(new UserPreferencesModel());

        // Assert
        Assert.That(result,Is.InstanceOf<UnauthorizedObjectResult>());
    }

    [Test]
    public async Task GetPreferences_ReturnsOk_WithPreferences()
    {
        // Arrange
        var expectedPreferences = new UserPreferencesModel { Language = LanguageEnum.English, Genre = GenreEnum.Action };
        _mockUserService.Setup(x => x.GetPreferences(1)).ReturnsAsync(expectedPreferences);

        // Act
        var result = await _controller.GetPreferences();

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value, Is.EqualTo(expectedPreferences));
    }

    [Test]
    public async Task GetPreferences_ReturnsNotFound_WhenPreferencesAreNull()
    {
        // Arrange
        _mockUserService.Setup(x => x.GetPreferences(1)).ReturnsAsync((UserPreferencesModel)null);

        // Act
        var result = await _controller.GetPreferences();

        // Assert
        Assert.That(result, Is.InstanceOf<NotFoundObjectResult>());
    }

    [Test]
    public async Task GetPreferences_ReturnsUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _controller.ControllerContext.HttpContext = new DefaultHttpContext(); // No user

        // Act
        var result = await _controller.GetPreferences();

        // Assert
        Assert.That(result,Is.InstanceOf<UnauthorizedObjectResult>());
    }
}