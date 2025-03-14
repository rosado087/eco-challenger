using Moq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EcoChallenger.Controllers;
using Microsoft.Extensions.Logging;

public class LoginGoogleTest
{
    private readonly LoginController _controller;
    private readonly Mock<IConfiguration> _mockConfig;
    private readonly AppDbContext _dbContext;

    public LoginGoogleTest()
    {
        var mockLogger = new Mock<ILogger<LoginController>>();
        _mockConfig = new Mock<IConfiguration>();

        // Setup in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDB")
            .Options;
        _dbContext = new AppDbContext(options);

        // Initialize the controller with dependencies
        _controller = new LoginController(_dbContext, _mockConfig.Object, mockLogger.Object);
    }

    [Fact]
    public void GetGoogleId_Returns_ClientId_From_Config()
    {
        // Arrange
        _mockConfig.Setup(c => c["GoogleClient:ClientId"]).Returns("test-client-id");

        // Act
        var result = _controller.GetGoogleId();

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        Assert.NotNull(successProperty);

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

        GAuthModel model = new GAuthModel {
            GoogleToken = "test-token",
            Email = "test@example.com"
        };

        // Act
        var result = await _controller.AuthenticateGoogle(model);

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
        GAuthModel model = new GAuthModel {
            GoogleToken = "invalid-token",
            Email = "nonexistent@example.com"
        };

        // Act
        var result = await _controller.AuthenticateGoogle(model);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        Assert.NotNull(successProperty);

        var successValue = (bool)successProperty.GetValue(result.Value);
        Assert.False(successValue);
    }
}
