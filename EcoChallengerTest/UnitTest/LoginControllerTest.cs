using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using EcoChallenger.Controllers;
using Microsoft.Extensions.Logging;
using EcoChallenger.Utils;
using EcoChallenger.Models;
using NUnit.Framework;
using System.Threading.Tasks;

namespace EcoChallengerTest.UnitTest
{
    public class LoginControllerTest
    {
        private LoginController? _controller;
        private AppDbContext? _dbContext;
        private Mock<IConfiguration>? _mockConfig;

        [SetUp]
        public void Setup()
        {
            var mockLogger = new Mock<ILogger<LoginController>>();

            // Set up in-memory database
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new AppDbContext(options);
            _mockConfig = new Mock<IConfiguration>();

            _controller = new LoginController(_dbContext, _mockConfig.Object, mockLogger.Object);

            //Setup JWT Settings
            var jwtSettings = new JwtSettings
            {
                Key = "ScretForTestingOnlyScretForTestingOnly",
                Issuer = "https://ecochallenger.duckdns.org",
                Audience = "https://ecochallenger.duckdns.org",
                TokenLifetime = 8
            };

            TokenManager.Initialize(jwtSettings);
        }

        [Test]
        public async Task Login_Returns_Token_When_Credentials_Are_Correct()
        {
            // Arrange
            var testUser = new User { 
                Email = "test58@example.com",
                Username = "TestUser58",
                Password = PasswordGenerator.GeneratePasswordHash("validPassword") 
            };

            _dbContext!.Users.Add(testUser);
            await _dbContext.SaveChangesAsync();

            var loginRequest = new LoginRequestModel { Email = "test58@example.com", Password = "validPassword" };

            // Act
            var result = await _controller!.Login(loginRequest);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var successProperty = result.Value?.GetType().GetProperty("success");
            Assert.That(successProperty, Is.Not.Null);

            var successValue = successProperty?.GetValue(result.Value);
            Assert.That(successValue, Is.True);

            var tokenProperty = result.Value?.GetType().GetProperty("token");
            Assert.That(tokenProperty, Is.Not.Null);
            Assert.That(tokenProperty!.GetValue(result.Value), Is.Not.Null);
        }

        [Test]
        public async Task Login_Fails_When_Email_Does_Not_Exist()
        {
            // Arrange
            var loginRequest = new LoginRequestModel { 
                Email = "nonexistent@example.com",
                Password = "password" 
            };

            // Act
            var result = await _controller!.Login(loginRequest) as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var successProperty = result.Value?.GetType().GetProperty("success");
            var messageProperty = result.Value?.GetType().GetProperty("message");

            Assert.That(successProperty, Is.Not.Null);
            Assert.That(messageProperty, Is.Not.Null);

            var successValue = successProperty?.GetValue(result.Value);
            Assert.That(successValue, Is.False);
        }

        [Test]
        public async Task Login_Fails_When_Password_Is_Incorrect()
        {
            // Arrange
            var testUser = new User {
                Email = "test@example.com", 
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            _dbContext!.Users.Add(testUser);
            await _dbContext.SaveChangesAsync();

            var loginRequest = new LoginRequestModel {
                Email = "test@example.com", 
                Password = "wrongPassword" 
            };

            // Act
            var result = await _controller!.Login(loginRequest) as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var successProperty = result.Value?.GetType().GetProperty("success");
            var messageProperty = result.Value?.GetType().GetProperty("message");

            Assert.That(successProperty, Is.Not.Null);
            Assert.That(messageProperty, Is.Not.Null);

            var successValue = successProperty?.GetValue(result.Value);
            Assert.That(successValue, Is.False);
        }

        [Test]
        public async Task Login_Fails_When_Email_Or_Password_Is_Missing()
        {
            // Arrange
            var loginRequest = new LoginRequestModel { Email = "", Password = "" };

            // Act
            var result = await _controller!.Login(loginRequest) as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var successProperty = result.Value?.GetType().GetProperty("success");
            var messageProperty = result.Value?.GetType().GetProperty("message");

            Assert.That(successProperty, Is.Not.Null);
            Assert.That(messageProperty, Is.Not.Null);

            var successValue = successProperty?.GetValue(result.Value);
            Assert.That(successValue, Is.False);
        }
    }
}