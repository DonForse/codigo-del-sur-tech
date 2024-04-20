using CodigoDelSurApi.Api.Models;
using CodigoDelSurApi.Api.Services;
using CodigoDelSurApi.Infrastructure;
using CodigoDelSurApi.Infrastructure.DataEntities;
using CodigoDelSurApi.Infrastructure.Enums;
using Microsoft.AspNet.Identity;
using MockQueryable.Moq;
using Moq;

namespace Tests.CodigoDelSur.Api.Services;

[TestFixture]
public class UserServiceTests
{
    private UserService _userService;
    private Mock<ICodigoDelSurDbContext> _mockContext;
    //private Mock<DbSet<User>> _mockUsers;
    //private Mock<DbSet<UserPreferences>> _mockUserPreferences;
    private Mock<IPasswordHasher> _mockPasswordHasher;

    [SetUp]
    public void Setup()
    {
        _mockContext = new Mock<ICodigoDelSurDbContext>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();

        _userService = new UserService(_mockContext.Object, _mockPasswordHasher.Object);
    }

    [Test]
    public async Task AddUserAsync_AddsUserCorrectly()
    {
        // Arrange
        string username = "newUser";
        string password = "password";
        string hashedPassword = "hashedPassword";

        _mockPasswordHasher.Setup(p => p.HashPassword(password)).Returns(hashedPassword);

        var users = new List<User>();
        var mockUserDbSet = users.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(x => x.Users).Returns(mockUserDbSet.Object);
        _mockContext.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);  // Simulate saving changes

        // Act
        await _userService.AddUserAsync(username, password);

        // Assert
        mockUserDbSet.Verify(x => x.Add(It.Is<User>(u => u.Username == username && u.PasswordHash == hashedPassword)), Times.Once);
        _mockContext.Verify(x => x.SaveChangesAsync(default), Times.Once);  // Verify that changes are saved
    }

    [Test]
    public async Task Exists_ReturnsTrue_WhenUserExists()
    {
        // Arrange
        string username = "existingUser";
        var users = new List<User>
        {
            new User { Username = "existingUser", UserId = 1 }
            // Add more users as needed for thorough testing
        };

        var mockUserDbSet = users.AsQueryable().BuildMockDbSet();
        _mockContext.Setup(x => x.Users).Returns(mockUserDbSet.Object);
        
        // Act
        var exists = await _userService.Exists(username);

        // Assert
        Assert.That(exists, Is.True);
    }


    [Test]
    public async Task GetPreferences_ReturnsPreferences_WhenFound()
    {
        // Arrange
        int userId = 1;

        var userPreferences = new List<UserPreferences>() { new UserPreferences { UserId = userId, Language = "en-US", Genre = 2 } };
        var mockUserPreferencesDbSet = userPreferences.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(x => x.UserPreferences).Returns(mockUserPreferencesDbSet.Object);

        // Act
        var result = await _userService.GetPreferences(userId);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Language, Is.EqualTo(LanguageEnum.English));
    }

    [Test]
    public async Task SavePreferences_SavesNewPreferences_WhenNotExisting()
    {
        // Arrange
        int userId = 1;
        var preferencesModel = new UserPreferencesModel { Language = LanguageEnum.English, Genre = GenreEnum.Action };

        var userPreferences = new List<UserPreferences>() { new UserPreferences { UserId = 5, Language = "en-US", Genre = 2 } };
        var mockUserPreferencesDbSet = userPreferences.AsQueryable().BuildMockDbSet();

        _mockContext.Setup(x => x.UserPreferences).Returns(mockUserPreferencesDbSet.Object);

        // Act
        await _userService.SavePreferences(userId, preferencesModel);

        // Assert
        mockUserPreferencesDbSet.Verify(x => x.Add(It.IsAny<UserPreferences>()), Times.Once);
        _mockContext.Setup(m => m.SaveChangesAsync(default)).ReturnsAsync(1);  // Simulate saving changes
    }
}