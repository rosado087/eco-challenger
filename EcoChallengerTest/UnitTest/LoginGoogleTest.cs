using Moq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EcoChallenger.Controllers;
using Microsoft.Extensions.Logging;
using EcoChallenger.Models;
using EcoChallenger.Utils;
using NUnit.Framework;

namespace EcoChallengerTest.UnitTest 
{
    public class LoginGoogleTest
    {
        private LoginController? _controller;
        private Mock<IConfiguration>? _mockConfig;
        private AppDbContext? _dbContext;

        [SetUp]
        public void Setup()
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
        public void GetGoogleId_Returns_ClientId_From_Config()
        {
            // Arrange
            _mockConfig!.Setup(c => c["GoogleClient:ClientId"]).Returns("test-client-id");

            // Act
            var result = _controller!.GetGoogleId();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var successProperty = result.Value?.GetType().GetProperty("clientId");
            Assert.That(successProperty, Is.Not.Null);

            var successValue = successProperty?.GetValue(result.Value) as string;
            Assert.That(successValue, Is.Not.Null);
            Assert.That(successValue, Is.EqualTo("test-client-id"));
        }

        [Test]
        public async Task AuthenticateGoogle_Returns_Success_When_User_Exists()
        {
            // Arrange
            var testUser = new User { Email = "test@example.com", Username = "test", Password = "123", GoogleToken = "test-token" };
            _dbContext!.Users.Add(testUser);
            await _dbContext.SaveChangesAsync();

            GAuthModel model = new GAuthModel {
                GoogleToken = "test-token",
                Email = "test@example.com"
            };

            // Act
            var result = await _controller!.AuthenticateGoogle(model);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var successProperty = result.Value?.GetType().GetProperty("success");
            Assert.That(successProperty, Is.Not.Null);

            var successValue = successProperty?.GetValue(result.Value);
            Assert.That(successValue, Is.True);
        }

        [Test]
        public async Task AuthenticateGoogle_Returns_True_When_User_Does_Not_Exist()
        {
            // Arrange
            GAuthModel model = new GAuthModel {
                GoogleToken = "invalid-token",
                Email = "nonexistent@example.com"
            };

            // Act
            var result = await _controller!.AuthenticateGoogle(model);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);

            var successProperty = result.Value?.GetType().GetProperty("success");
            Assert.That(successProperty, Is.Not.Null);

            var successValue = successProperty?.GetValue(result.Value);
            Assert.That(successValue, Is.True);
        }
    }
}
