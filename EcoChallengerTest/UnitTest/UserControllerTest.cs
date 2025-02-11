using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EcoChallenger.Controllers;
using Newtonsoft.Json.Linq;

public class UserControllerTest
{
    private readonly UserController _controller;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AppDbContext _dbContext;

    public UserControllerTest()
    {
        _mockConfig = new Mock<IConfiguration>();

        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;
        _dbContext = new AppDbContext(options);

        // Initialize the controller with dependencies
        _controller = new UserController(_dbContext, _mockConfig.Object);
    }

    [Fact]
    public async Task GetGoogleId_Returns_ClientId_From_Config()
    {
        // Arrange
        _mockConfig.Setup(c => c["GoogleClient:ClientId"]).Returns("test-client-id");

        // Act
        var result = await _controller.GetGoogleId() as JsonResult;

        // Assert result is not null
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        // Extract 'success' property using reflection
        var successProperty = result.Value.GetType().GetProperty("success");
        Assert.NotNull(successProperty); // Ensure the property exists

        var successValue = successProperty.GetValue(result.Value) as string;
        Assert.NotNull(successValue);
        Assert.Equal("test-client-id", successValue);
    }

    [Fact]
    public async Task AuthenticateGoogle_Returns_Success_When_User_Exists()
    {
        // Arrange
        var testUser = new User { Email = "test@example.com", Username = "test", Password = "123", GoogleToken = "test-token" };
        _dbContext.Users.Add(testUser);
        await _dbContext.SaveChangesAsync();

        string[] values = { "test-token", "test@example.com" };

        // Act
        var result = await _controller.AuthenticateGoogle(values) as JsonResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        Assert.NotNull(successProperty);

        var successValue = (bool)successProperty.GetValue(result.Value);
        Assert.True(successValue);
    }

    [Fact]
    public async Task AuthenticateGoogle_Returns_False_When_User_Does_Not_Exist()
    {
        // Arrange
        string[] values = { "invalid-token", "nonexistent@example.com" };

        // Act
        var result = await _controller.AuthenticateGoogle(values) as JsonResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        Assert.NotNull(successProperty);

        var successValue = (bool)successProperty.GetValue(result.Value);
        Assert.False(successValue);
    }

    [Fact]
    public async Task SignUpGoogle_Creates_New_User_And_Returns_Success()
    {
        // Arrange
        string[] values = { "TestUser", "test@example.com", "test-token" };

        // Act
        var result = await _controller.SignUpGoogle(values) as JsonResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        Assert.NotNull(successProperty);

        var successValue = (bool)successProperty.GetValue(result.Value);
        Assert.True(successValue);

        var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == "test@example.com");
        Assert.NotNull(user);
        Assert.Equal("TestUser", user.Username);
        Assert.Equal("test-token", user.GoogleToken);
    }

    [Fact]
    public async Task UserExists_Returns_True_When_User_Exists()
    {
        // Arrange
        var testUser = new User { Email = "test@example.com", Username = "ExistingUser", Password = "123", GoogleToken = "test-token" };
        _dbContext.Users.Add(testUser);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _controller.UserExists("ExistingUser") as JsonResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        Assert.NotNull(successProperty);

        var successValue = (bool)successProperty.GetValue(result.Value);
        Assert.True(successValue);
    }
}
