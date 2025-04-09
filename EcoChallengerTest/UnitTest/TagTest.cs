using EcoChallenger.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EcoChallengerTest.UnitTest
{
    public class TagTest
    {
        private AppDbContext? _context;
        private TagController? _controller;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppDbContext(options);

            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(Directory.GetCurrentDirectory());

            var loggerMock = new Mock<ILogger<LoginController>>();

            _controller = new TagController(_context, envMock.Object, loggerMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, "Admin")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        private TagController CreateControllerWithContext(bool isAdmin)
        {
            var envMock = new Mock<IWebHostEnvironment>();
            envMock.Setup(e => e.WebRootPath).Returns(Directory.GetCurrentDirectory());

            var loggerMock = new Mock<ILogger<LoginController>>();
            var controller = new TagController(_context!, envMock.Object, loggerMock.Object);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Role, isAdmin ? "Admin" : "User")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            return controller;
        }

        [Test]
        public async Task CreateTag_Succeeds_If_Admin()
        {
            var controller = CreateControllerWithContext(isAdmin: true);

            var model = new TagCRUDModel
            {
                Name = "Eco Warriorsssss",
                BackgroundColor = "#fffddd",
                TextColor = "#000000",
                Style = Tag.TagStyle.NORMAL,
                Price = 100
            };

            var result = await controller.CreateTag(model) as OkObjectResult;
            Assert.That(result, Is.Not.Null);
            
            var successValue = result?.Value?.GetType().GetProperty("success")?.GetValue(result.Value);
            Assert.That(successValue, Is.True);
        }

        [Test]
        public async Task Cannot_Add_Duplicate_Tag_Name()
        {
            _context!.Tags.Add(new Tag
            {
                Name = "Eco Warrior",
                BackgroundColor = "#000",
                TextColor = "#fff",
                Style = Tag.TagStyle.NORMAL,
                Price = 100
            });
            _context.SaveChanges();

            var controller = CreateControllerWithContext(isAdmin: true);

            var model = new TagCRUDModel
            {
                Name = "Eco Warrior",
                BackgroundColor = "#fff",
                TextColor = "#000",
                Style = Tag.TagStyle.NORMAL,
                Price = 50
            };

            var result = await controller.CreateTag(model);
            var badRequest = (BadRequestObjectResult)result;
            Assert.That(badRequest.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task EditTag_Updates_Existing_Tag()
        {
            // Arrange
            var tag = new Tag
            {
                Name = "OldTag",
                BackgroundColor = "#000000",
                TextColor = "#FFFFFF",
                Style = Tag.TagStyle.NORMAL,
                Price = 10
            };
            _context!.Tags.Add(tag);
            await _context.SaveChangesAsync();

            var model = new TagCRUDModel
            {
                Name = "UpdatedTag",
                BackgroundColor = "#111111",
                TextColor = "#EEEEEE",
                Price = 20,
                Style = Tag.TagStyle.SOFT
            };

            // Act
            var result = await _controller!.EditTag(tag.Id, model) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);

            var successValue = result!.Value?.GetType().GetProperty("success")?.GetValue(result.Value);
            Assert.That(successValue, Is.True);

            var updatedTag = await _context.Tags.FindAsync(tag.Id);

            Assert.That(updatedTag, Is.Not.Null);
            Assert.That(updatedTag!.Name, Is.EqualTo("UpdatedTag"));
            Assert.That(updatedTag.BackgroundColor, Is.EqualTo("#111111"));
            Assert.That(updatedTag.TextColor, Is.EqualTo("#EEEEEE"));
            Assert.That(updatedTag.Price, Is.EqualTo(20));
            Assert.That(updatedTag.Style, Is.EqualTo(Tag.TagStyle.SOFT));
        }

        [Test]
        public void RemoveTag_Deletes_Tag()
        {
            // Arrange
            var tag = new Tag
            {
                Name = "DeleteTag",
                BackgroundColor = "#123456",
                TextColor = "#654321",
                Style = Tag.TagStyle.NORMAL,
                Price = 15
            };
            _context!.Tags.Add(tag);
            _context.SaveChanges();

            // Act
            var result = _controller!.RemoveTag(tag.Id) as OkObjectResult;

            // Assert
            Assert.That(result, Is.Not.Null);

            var successValue = result?.Value?.GetType().GetProperty("success")?.GetValue(result.Value);
            Assert.That(successValue, Is.True);
            Assert.That(_context.Tags.Any(t => t.Id == tag.Id), Is.False);
        }

        [Test]
        public void GetTag_Returns_Tag()
        {
            // Arrange
            var tag = new Tag
            {
                Name = "GetMe",
                BackgroundColor = "#222",
                TextColor = "#ddd",
                Style = Tag.TagStyle.NORMAL,
                Price = 30
            };
            _context!.Tags.Add(tag);
            _context.SaveChanges();

            // Act
            var result = _controller!.GetTag(tag.Id) as OkObjectResult;
            Assert.That(result, Is.Not.Null);

            // Assert
            var returnedTag = result!.Value as Tag;
            Assert.That(returnedTag, Is.Not.Null);
            Assert.That(tag.Name, Is.EqualTo(returnedTag!.Name));
            Assert.That(tag.Price, Is.EqualTo(returnedTag.Price));
        }
    }
}
