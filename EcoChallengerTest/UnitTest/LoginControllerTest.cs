using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using EcoChallenger.Controllers;

public class LoginControllerTest
{
    private readonly LoginController _controller;
    private readonly AppDbContext _dbContext;
    private readonly Mock<IConfiguration> _mockConfig;

    public LoginControllerTest()
    {
        // Set up in-memory database
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;

        _dbContext = new AppDbContext(options);
        _mockConfig = new Mock<IConfiguration>();

        _controller = new LoginController(_dbContext, _mockConfig.Object);
    }

    [Fact]
    public async Task Login_Returns_Token_When_Credentials_Are_Correct()
    {
        // Arrange
        var testUser = new User { 
            Email = "test@example.com",
            Username = "TestUser",
            Password = PasswordGenerator.GeneratePasswordHash("validPassword") 
        };

        _dbContext.Users.Add(testUser);
        await _dbContext.SaveChangesAsync();

        var loginRequest = new LoginRequestModel { Email = "test@example.com", Password = "validPassword" };

        // Act
        var result = await _controller.Login(loginRequest) as JsonResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        Assert.NotNull(successProperty);

        var successValue = (bool)successProperty.GetValue(result.Value);
        Assert.True(successValue);

        var tokenProperty = result.Value.GetType().GetProperty("token");
        Assert.NotNull(tokenProperty);
        Assert.NotNull(tokenProperty.GetValue(result.Value));
    }

    [Fact]
    public async Task Login_Fails_When_Email_Does_Not_Exist()
    {
        // Arrange
        var loginRequest = new LoginRequestModel { 
            Email = "nonexistent@example.com",
            Password = "password" 
        };

        // Act
        var result = await _controller.Login(loginRequest) as JsonResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        var messageProperty = result.Value.GetType().GetProperty("message");

        Assert.NotNull(successProperty);
        Assert.NotNull(messageProperty);

        var successValue = (bool)successProperty.GetValue(result.Value);
        var messageValue = (string)messageProperty.GetValue(result.Value);

        Assert.False(successValue);
        Assert.Equal("Invalid email or password.", messageValue);
    }

    [Fact]
    public async Task Login_Fails_When_Password_Is_Incorrect()
    {
        // Arrange
        var testUser = new User {
            Email = "test@example.com", 
            Username = "test",
            Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
        };

        _dbContext.Users.Add(testUser);
        await _dbContext.SaveChangesAsync();

        var loginRequest = new LoginRequestModel {
            Email = "test@example.com", 
            Password = "wrongPassword" 
        };

        // Act
        var result = await _controller.Login(loginRequest) as JsonResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        var messageProperty = result.Value.GetType().GetProperty("message");

        Assert.NotNull(successProperty);
        Assert.NotNull(messageProperty);

        var successValue = (bool)successProperty.GetValue(result.Value);
        var messageValue = (string)messageProperty.GetValue(result.Value);

        Assert.False(successValue);
        Assert.Equal("Invalid email or password.", messageValue);
    }

    [Fact]
    public async Task Login_Fails_When_Email_Or_Password_Is_Missing()
    {
        // Arrange
        var loginRequest = new LoginRequestModel { Email = "", Password = "" };

        // Act
        var result = await _controller.Login(loginRequest) as JsonResult;

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Value);

        var successProperty = result.Value.GetType().GetProperty("success");
        var messageProperty = result.Value.GetType().GetProperty("message");

        Assert.NotNull(successProperty);
        Assert.NotNull(messageProperty);

        var successValue = (bool)successProperty.GetValue(result.Value);
        var messageValue = (string)messageProperty.GetValue(result.Value);

        Assert.False(successValue);
        Assert.Equal("Email and password are required.", messageValue);
    }
}
