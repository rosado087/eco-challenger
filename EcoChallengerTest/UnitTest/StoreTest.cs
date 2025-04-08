using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using EcoChallenger.Controllers;
using NUnit.Framework;

namespace EcoChallengerTest.UnitTest
{
    public class StoreControllerTest
    {
        private readonly StoreController _controller;
        private readonly AppDbContext _dbContext;
        private readonly Mock<ILogger<LoginController>> _mockLogger;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;
        private readonly User _testUser;

        public StoreControllerTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new AppDbContext(options);
            _mockLogger = new Mock<ILogger<LoginController>>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();

            _testUser = new User
            {
                Username = "testuser",
                Email = "test@example.com",
                Points = 100
            };

            _dbContext.Users.Add(_testUser);
            _dbContext.SaveChanges();

            var claims = new List<Claim>
            {
                new Claim("userid", _testUser.Id.ToString()),
                new Claim("username", _testUser.Username),
                new Claim(ClaimTypes.Email, _testUser.Email),
                new Claim("isAdmin", "false")
            };

            var identity = new ClaimsIdentity(claims, "mock");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpContext = new DefaultHttpContext
            {
                User = claimsPrincipal
            };

            _httpContextAccessor.Setup(x => x.HttpContext).Returns(mockHttpContext);

            UserContext.Initialize(_httpContextAccessor.Object);

            _controller = new StoreController(_dbContext, _mockLogger.Object);
        }

        [Test]
        public async Task PurchaseTag_Succeeds_When_User_Has_Enough_Points()
        {
            // Arrange
            var tag = new Tag { Name = "Eco", Price = 50, BackgroundColor = "#000", TextColor = "#fff", Style = Tag.TagStyle.NORMAL };
            _dbContext.Tags.Add(tag);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = _controller.PurchaseTag(tag.Id) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var success = result!.Value?.GetType().GetProperty("success")?.GetValue(result.Value);
            
            Assert.That(success, Is.True);
            Assert.That(_dbContext.TagUsers.Count(), Is.EqualTo(1));
            Assert.That(_dbContext.Users.First().Points, Is.EqualTo(50));
        }

        [Test]
        public async Task PurchaseTag_Fails_When_Already_Owned()
        {
            // Arrange
            var tag = new Tag { Name = "Eco", Price = 30, BackgroundColor = "#000", TextColor = "#fff", Style = Tag.TagStyle.NORMAL };

            _dbContext.Tags.Add(tag);
            _dbContext.TagUsers.Add(new TagUsers { Tag = tag, User = _testUser, SelectedTag = false });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = _controller.PurchaseTag(tag.Id) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var success = result!.Value?.GetType().GetProperty("success")?.GetValue(result.Value);
            Assert.That(success, Is.False);
        }

        [Test]
        public async Task PurchaseTag_Fails_When_Not_Enough_Points()
        {
            // Arrange
            var tag = new Tag { Name = "Expensive", Price = 200, BackgroundColor = "#000", TextColor = "#fff", Style = Tag.TagStyle.NORMAL };
            _dbContext.Tags.Add(tag);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = _controller.PurchaseTag(tag.Id) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var success = result!.Value?.GetType().GetProperty("success")?.GetValue(result.Value);
            Assert.That(success, Is.False);
        }
    }
}
