using EcoChallenger.Controllers;
using EcoChallenger.Models;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System.Linq.Expressions;
using System.Security.Claims;

namespace EcoChallengerTest.UnitTest
{
    public class ChallengesTest
    {
        private AppDbContext _dbContext;
        private Mock<ILogger<GamificationController>> _loggerMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new AppDbContext(options);
            _loggerMock = new Mock<ILogger<GamificationController>>();
        }

        private (bool success, string message) ExtractJson(JsonResult result)
        {
            var value = result!.Value!;
            var success = (bool)value.GetType().GetProperty("success")!.GetValue(value)!;
            var message = (string)value.GetType().GetProperty("message")!.GetValue(value)!;
            return (success, message);
        }

        [Test]
        public async Task CreateChallenge_Success()
        {
            var model = new ChallengeModel { Title = "Test", Description = "Desc", Points = 10, Type = "Daily" };
            var controller = new ChallengeController(_dbContext, _loggerMock.Object);

            var result = await controller.CreateChallenge(model) as JsonResult;
            var (success, message) = ExtractJson(result!);

            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Desafio criado com sucesso"));
        }

        [Test]
        public async Task CreateChallenge_Fails_WhenDuplicate()
        {
            _dbContext.Challenges.Add(new Challenge { Title = "Dup", Description = "Desc", Type = "Daily", Points = 5 });
            _dbContext.SaveChanges();
            var model = new ChallengeModel { Title = "Dup", Description = "NewDesc", Type = "Daily", Points = 5 };

            var controller = new ChallengeController(_dbContext, _loggerMock.Object);
            var result = await controller.CreateChallenge(model) as JsonResult;
            var (success, message) = ExtractJson(result!);

            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("Já existe um desafio com esse titulo ou descrição"));
        }

        [Test]
        public void GetAllChallenges_ReturnsAll()
        {
            _dbContext.Challenges.AddRange(
                new Challenge { Title = "A", Description = "testA", Type = "Daily", Points = 5 },
                new Challenge { Title = "B", Description = "testB", Type = "Weekly", Points = 25 }
            );
            _dbContext.SaveChanges();
            var controller = new ChallengeController(_dbContext, _loggerMock.Object);

            var result = controller.GetAllChallenges(null) as OkObjectResult;
            var list = result!.Value as List<Challenge>;
            Assert.That(list, Has.Count.EqualTo(2));
        }

        [Test]
        public void GetAllChallenges_ReturnsFiltered()
        {
            _dbContext.Challenges.AddRange(
                new Challenge { Title = "Test", Description = "test", Points = 5, Type = "Daily" },
                new Challenge { Title = "Another", Description = "another", Points = 15, Type = "Weekly" }
            );
            _dbContext.SaveChanges();
            var controller = new ChallengeController(_dbContext, _loggerMock.Object);

            var result = controller.GetAllChallenges("Test") as OkObjectResult;
            var list = result!.Value as List<Challenge>;
            Assert.That(list, Has.Count.EqualTo(1));
        }

        [Test]
        public async Task EditChallenge_Success()
        {
            var chal = new Challenge { Title = "Old", Description = "Desc", Points = 10, Type = "Daily" };
            _dbContext.Challenges.Add(chal);
            _dbContext.SaveChanges();

            var model = new ChallengeModel { Title = "Old", Description = "Desc", Points = 20, Type = "Daily" };
            var controller = new ChallengeController(_dbContext, _loggerMock.Object);

            var result = await controller.EditChallenge(model, chal.Id) as JsonResult;
            var (success, message) = ExtractJson(result!);

            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Desafio editado com sucesso"));
        }

        [Test]
        public async Task EditChallenge_Fails_WhenNotFound()
        {
            var model = new ChallengeModel { Title = "DoesNotExist", Description = "none", Type = "Daily", Points = 4 };
            var controller = new ChallengeController(_dbContext, _loggerMock.Object);

            var result = await controller.EditChallenge(model, 999) as JsonResult;
            var (success, message) = ExtractJson(result!);

            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("O desafio não existe"));
        }

        [Test]
        public async Task EditChallenge_Fails_WhenDuplicateTitleExists()
        {
            // Arrange
            var original = new Challenge { Title = "Original", Description = "Desc1", Type = "Daily", Points = 5 };
            var other = new Challenge { Title = "DuplicateTitle", Description = "Desc2", Type = "Weekly", Points = 15 };

            _dbContext.Challenges.AddRange(original, other);
            _dbContext.SaveChanges();

            var model = new ChallengeModel { Title = "DuplicateTitle", Description = "Updated", Points = 15, Type = "Weekly" };

            var controller = new ChallengeController(_dbContext, _loggerMock.Object);

            // Act
            var result = await controller.EditChallenge(model, original.Id) as JsonResult;
            var (success, message) = ExtractJson(result!);

            // Assert
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("Já existe um desafio com este título"));
        }

        [Test]
        public async Task EditChallenge_Fails_WhenDuplicateDescriptionExists()
        {
            // Arrange
            var original = new Challenge { Title = "Original", Description = "KeepThis", Type = "Daily", Points = 5 };
            var other = new Challenge { Title = "Different", Description = "DuplicateDescription", Type = "Weekly", Points = 20 };

            _dbContext.Challenges.AddRange(original, other);
            _dbContext.SaveChanges();

            var model = new ChallengeModel { Title = "Updated Title", Description = "DuplicateDescription", Points = 20, Type = "Weekly" };

            var controller = new ChallengeController(_dbContext, _loggerMock.Object);

            // Act
            var result = await controller.EditChallenge(model, original.Id) as JsonResult;
            var (success, message) = ExtractJson(result!);

            // Assert
            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("Já existe um desafio com esta descrição"));
        }

        [Test]
        public async Task DeleteChallenge_Success()
        {
            var chal = new Challenge { Title = "DelMe", Description = "del", Points = 5, Type = "Daily" };
            _dbContext.Challenges.Add(chal);
            _dbContext.SaveChanges();
            var controller = new ChallengeController(_dbContext, _loggerMock.Object);

            var result = await controller.DeleteChallenge(chal.Id) as JsonResult;
            var (success, message) = ExtractJson(result!);

            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Desafio removido com sucesso"));
        }

        [Test]
        public async Task DeleteChallenge_Fails_WhenNotFound()
        {
            var controller = new ChallengeController(_dbContext, _loggerMock.Object);
            var result = await controller.DeleteChallenge(999) as JsonResult;
            var (success, message) = ExtractJson(result!);

            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("O desafio não existe."));
        }

        [Test]
        public async Task GetChallenge_Success()
        {
            var chal = new Challenge { Title = "GetMe", Description = "get", Points = 5, Type = "Daily" };
            _dbContext.Challenges.Add(chal);
            _dbContext.SaveChanges();
            var controller = new ChallengeController(_dbContext, _loggerMock.Object);

            var result = await controller.GetChallenge(chal.Id) as JsonResult;
            var (success, message) = ExtractJson(result!);

            Assert.That(success, Is.True);
            Assert.That(message, Is.EqualTo("Desafio encontrado com sucesso"));
        }

        [Test]
        public async Task GetChallenge_Fails_WhenNotFound()
        {
            var controller = new ChallengeController(_dbContext, _loggerMock.Object);
            var result = await controller.GetChallenge(999) as JsonResult;
            var (success, message) = ExtractJson(result!);

            Assert.That(success, Is.False);
            Assert.That(message, Is.EqualTo("O desafio não existe"));
        }
    }
}
