using EcoChallenger.Controllers;
using EcoChallenger.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace EcoChallengerTest.UnitTest
{
    public class TagTest
    {
        private readonly AppDbContext _context;
        private readonly TagController _controller;

        public TagTest()
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
            var controller = new TagController(_context, envMock.Object, loggerMock.Object);

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

        [Fact]
        public async Task CreateTag_Succeeds_If_Admin()
        {
            var controller = CreateControllerWithContext(isAdmin: true);

            var model = new TagCRUDModel
            {
                Name = "Eco Warrior",
                BackgroundColor = "#fff",
                TextColor = "#000",
                Style = Tag.TagStyle.NORMAL,
                Price = 100
            };

            var result = await controller.CreateTag(model) as OkObjectResult;
            Assert.NotNull(result);
            Assert.True((bool)result.Value.GetType().GetProperty("success")?.GetValue(result.Value));
        }

        [Fact]
        public async Task Cannot_Add_Duplicate_Tag_Name()
        {
            _context.Tags.Add(new Tag
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
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequest.StatusCode);
        }

        [Fact]
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
            _context.Tags.Add(tag);
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
            var result = await _controller.EditTag(tag.Id, model) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.True((bool)result.Value.GetType().GetProperty("success")?.GetValue(result.Value));

            var updatedTag = await _context.Tags.FindAsync(tag.Id);
            Assert.Equal("UpdatedTag", updatedTag.Name);
            Assert.Equal("#111111", updatedTag.BackgroundColor);
            Assert.Equal("#EEEEEE", updatedTag.TextColor);
            Assert.Equal(20, updatedTag.Price);
            Assert.Equal(Tag.TagStyle.SOFT, updatedTag.Style);
        }

        [Fact]
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
            _context.Tags.Add(tag);
            _context.SaveChanges();

            // Act
            var result = _controller.RemoveTag(tag.Id) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.True((bool)result.Value.GetType().GetProperty("success")?.GetValue(result.Value));
            Assert.False(_context.Tags.Any(t => t.Id == tag.Id));
        }

        [Fact]
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
            _context.Tags.Add(tag);
            _context.SaveChanges();

            // Act
            var result = _controller.GetTag(tag.Id) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returnedTag = result.Value as Tag;
            Assert.NotNull(returnedTag);
            Assert.Equal(tag.Name, returnedTag.Name);
            Assert.Equal(tag.Price, returnedTag.Price);
        }
    }
}
