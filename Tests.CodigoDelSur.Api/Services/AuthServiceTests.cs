using CodigoDelSurApi.Api.Services;
using CodigoDelSurApi.Infrastructure;
using CodigoDelSurApi.Infrastructure.DataEntities;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using Tests.CodigoDelSur.Api.MoqDbHelper;

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

    public static Mock<DbSet<T>> CreateMockDbSet<T>(List<T> sourceList) where T : class
    {
        var queryable = sourceList.AsQueryable();
        var mockSet = new Mock<DbSet<T>>();

        // Setting up the provider to handle async operations
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(new TestAsyncQueryProvider<T>(queryable.Provider));
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        return mockSet;
    }
}