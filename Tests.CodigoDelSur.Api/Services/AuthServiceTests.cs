using CodigoDelSurApi.Api.Services;
using CodigoDelSurApi.Infrastructure;
using CodigoDelSurApi.Infrastructure.DataEntities;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;

namespace Tests.CodigoDelSur.Api.Services;

[TestFixture]
public class AuthServiceTests
{
    private AuthService _authService;
    private Mock<ICodigoDelSurDbContext> _mockContext;
    private Mock<IPasswordHasher> _mockPasswordHasher;

    [SetUp]
    public void Setup()
    {
        _mockContext = new Mock<ICodigoDelSurDbContext>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();

        var users = new List<User>
        {
            new User { UserId = 1, Username = "existingUser", PasswordHash = "hashedPassword" }
        };

        _mockContext.Setup(m => m.Users).Returns(users.AsQueryable().BuildMockDbSet().Object);

        _authService = new AuthService(_mockContext.Object, null, _mockPasswordHasher.Object);
    }

    [Test]
    public async Task AuthenticateUser_ReturnsNull_WhenUserNotFound()
    {
        // Act
        var result = await _authService.AuthenticateUser("nonexistentUser", "anyPassword");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task AuthenticateUser_ReturnsNull_WhenPasswordDoesNotMatch()
    {
        // Arrange
        _mockPasswordHasher.Setup(p => p.VerifyHashedPassword("hashedPassword", "wrongPassword"))
            .Returns(PasswordVerificationResult.Failed);

        // Act
        var result = await _authService.AuthenticateUser("existingUser", "wrongPassword");

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task AuthenticateUser_ReturnsUser_WhenCredentialsAreValid()
    {
        // Arrange
        _mockPasswordHasher.Setup(p => p.VerifyHashedPassword("hashedPassword", "correctPassword"))
            .Returns(PasswordVerificationResult.Success);

        // Act
        var result = await _authService.AuthenticateUser("existingUser", "correctPassword");

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Username, Is.EqualTo("existingUser"));
       
    }
}