using EcoChallenger.Controllers;
using EcoChallenger.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace EcoChallengerTest.UnitTest
{
    public class UsersTest
    {
        private AppDbContext _dbContext;
        private Mock<IWebHostEnvironment> _envMock;
        private Mock<ILogger<UsersController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new AppDbContext(options);
            _envMock = new Mock<IWebHostEnvironment>();
            _loggerMock = new Mock<ILogger<UsersController>>();
        }

        private void SetupUserContext(int userId, bool isAdmin = true)
        {
            var claims = new List<Claim>
            {
                new Claim("userid", userId.ToString())
            };

            if (isAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));

            var httpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "mock"))
            };

            var accessor = new Mock<IHttpContextAccessor>();
            accessor.Setup(x => x.HttpContext).Returns(httpContext);
            UserContext.Initialize(accessor.Object);
        }

        [Test]
        public void GetUsers_ReturnsAllUsers_ExceptSelf_WhenNoSearch()
        {
            // Arrange
            var adminUser = new User { Id = 1, Username = "admin", Email="admin@example.com", IsAdmin = true };
            var user1 = new User { Id = 2, Username = "user1", Email="user1@example.com" };
            var user2 = new User { Id = 3, Username = "user2", Email="user2@example.com" };

            _dbContext.Users.AddRange(adminUser, user1, user2);
            _dbContext.SaveChanges();

            SetupUserContext(adminUser.Id);

            var controller = new UsersController(_dbContext, _envMock.Object, _loggerMock.Object);

            // Act
            var result = controller.GetUsers(null) as OkObjectResult;

            // Assert
            var users = result!.Value as IEnumerable<object>;
            Assert.That(users, Is.Not.Null);
            Assert.That(users.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetUsers_ReturnsFilteredUsers_ThatContainSearch()
        {
            // Arrange
            var adminUser = new User { Id = 1, Username = "admin", Email="admin@example.com", IsAdmin = true };
            var user1 = new User { Id = 2, Username = "Diogo", Email="diogo@example.com" };
            var user2 = new User { Id = 3, Username = "Diana", Email="diana@example.com" };
            var user3 = new User { Id = 4, Username = "Carlos", Email="carlos@example.com" };

            _dbContext.Users.AddRange(adminUser, user1, user2, user3);
            _dbContext.SaveChanges();

            SetupUserContext(adminUser.Id);

            var controller = new UsersController(_dbContext, _envMock.Object, _loggerMock.Object);

            // Act
            var result = controller.GetUsers("Di") as OkObjectResult;

            // Assert
            var users = result!.Value as IEnumerable<object>;
            Assert.That(users, Is.Not.Null);
            Assert.That(users.Count(), Is.EqualTo(2));
        }

        [Test]
        public async Task ChangeBlock_ReturnsError_IfUserNotFound()
        {
            var controller = new UsersController(_dbContext, _envMock.Object, _loggerMock.Object);

            var result = await controller.ChangeBlock(999) as JsonResult;
            var json = result!.Value;

            var success = (bool)json.GetType().GetProperty("success")!.GetValue(json)!;
            var message = (string)json.GetType().GetProperty("message")!.GetValue(json)!;

            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("Este user não existe."));
        }

        [Test]
        public async Task ChangeBlock_TogglesUserBlockState()
        {
            var user = new User { Id = 10, Username = "target", Email="target@example.com", IsBlocked = false };
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();

            var controller = new UsersController(_dbContext, _envMock.Object, _loggerMock.Object);

            var result = await controller.ChangeBlock(user.Id) as JsonResult;
            var json = result!.Value;

            var success = (bool)json.GetType().GetProperty("success")!.GetValue(json)!;
            var message = (string)json.GetType().GetProperty("message")!.GetValue(json)!;

            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Estado mudado com successo."));

            var updatedUser = await _dbContext.Users.FindAsync(user.Id);
            Assert.That(updatedUser, Is.Not.Null);
            Assert.That(updatedUser!.IsBlocked, Is.True);
        }
    }
}
