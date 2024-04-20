using CodigoDelSurApi.Api.Controllers;
using CodigoDelSurApi.Api.Models;
using CodigoDelSurApi.Api.Services;
using CodigoDelSurApi.Infrastructure.DataEntities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.CodigoDelSur.Api.Controllers;

[TestFixture]
public class AuthControllerTests
{
    private AuthController _controller;
    private Mock<IAuthService> _mockAuthService;
    private Mock<IUserService> _mockUserService;
    private Mock<IConfiguration> _mockConfiguration;

    [SetUp]
    public void Setup()
    {
        _mockAuthService = new Mock<IAuthService>();
        _mockUserService = new Mock<IUserService>();
        _mockConfiguration = new Mock<IConfiguration>();

        _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("a very long and secure key that has more than 256 bytes");

        _controller = new AuthController(
            _mockAuthService.Object,
            _mockConfiguration.Object,
            _mockUserService.Object
        );
    }

    [Test]
    public async Task Register_ReturnsBadRequest_WhenUsernameExists()
    {
        // Arrange
        var userModel = new UserModel { Username = "existingUser", Password = "password123" };
        _mockUserService.Setup(x => x.Exists(userModel.Username)).ReturnsAsync(true);

        // Act
        var result = await _controller.Register(userModel);

        // Assert
        Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        var badRequestResult = result as BadRequestObjectResult;
        Assert.That(badRequestResult.Value, Is.EqualTo("Username is already taken."));
    }

    [Test]
    public async Task Register_ReturnsOk_WhenUserIsSuccessfullyRegistered()
    {
        // Arrange
        var userModel = new UserModel { Username = "newUser", Password = "password123" };
        _mockUserService.Setup(x => x.Exists(userModel.Username)).ReturnsAsync(false);

        // Act
        var result = await _controller.Register(userModel);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
    }

    [Test]
    public async Task Login_ReturnsUnauthorized_WhenCredentialsAreInvalid()
    {
        // Arrange
        var userModel = new UserModel { Username = "user", Password = "wrongPassword" };
        _mockAuthService.Setup(x => x.AuthenticateUser(userModel.Username, userModel.Password)).ReturnsAsync((User)null);

        // Act
        var result = await _controller.Login(userModel);

        // Assert
        Assert.That(result, Is.InstanceOf<UnauthorizedObjectResult>());
    }

    [Test]
    public async Task Login_ReturnsOk_WithToken_WhenCredentialsAreValid()
    {
        // Arrange
        var userModel = new UserModel { Username = "validUser", Password = "password123" };
        var user = new User { UserId = 1, Username = userModel.Username };
        _mockAuthService.Setup(x => x.AuthenticateUser(userModel.Username, userModel.Password)).ReturnsAsync(user);

        // Act
        var result = await _controller.Login(userModel);

        // Assert
        Assert.That(result, Is.InstanceOf<OkObjectResult>());
        var okResult = result as OkObjectResult;
        Assert.That(okResult.Value.ToString().Contains("Token"), Is.True);
    }
}