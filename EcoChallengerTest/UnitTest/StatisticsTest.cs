using EcoChallenger.Controllers;
using EcoChallenger.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace EcoChallengerTest.UnitTest
{
    public class StatisticsTest
    {
        private AppDbContext _dbContext;
        private Mock<ILogger<LoginController>> _loggerMock;
        private Mock<IWebHostEnvironment> _envMock;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new AppDbContext(options);
            _loggerMock = new Mock<ILogger<LoginController>>();
            _envMock = new Mock<IWebHostEnvironment>();
        }

        [Test]
        public void GetTopPurchasedTags_ReturnsOrderedList()
        {
            _dbContext.TagUsers.RemoveRange(_dbContext.TagUsers);
            _dbContext.Tags.RemoveRange(_dbContext.Tags);
            _dbContext.Users.RemoveRange(_dbContext.Users);
            _dbContext.SaveChanges();

            var tag = new Tag { Name = "Eco", Price = 10, TextColor = "#000000", BackgroundColor = "#fffddd" };
            var user = new User { Username = "TestUser", Email = "testuser@example.com" };
            _dbContext.Tags.Add(tag);
            _dbContext.Users.Add(user);
            _dbContext.TagUsers.Add(new TagUsers { Tag = tag, User = user });
            _dbContext.SaveChanges();

            var controller = new StatisticsController(_dbContext, _envMock.Object, _loggerMock.Object);
            var result = controller.GetTopPurchasedTags() as OkObjectResult;

            var value = result!.Value!;
            var tagsProperty = value.GetType().GetProperty("tags");
            var tags = tagsProperty!.GetValue(value) as IEnumerable<object>;

            Assert.That(result, Is.Not.Null);
            Assert.That(tags, Is.Not.Null);
            Assert.That(tags!.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetTopCompletedChallenges_ReturnsOrderedList()
        {
            _dbContext.UserChallenges.RemoveRange(_dbContext.UserChallenges);
            _dbContext.Challenges.RemoveRange(_dbContext.Challenges);
            _dbContext.Users.RemoveRange(_dbContext.Users);
            _dbContext.SaveChanges();

            var challenge = new Challenge { Title = "Challenge 1", Description = "test", Type = "Daily", Points = 10 };
            var user = new User { Username = "TestUser", Email = "testuser@example.com" };
            _dbContext.Challenges.Add(challenge);
            _dbContext.Users.Add(user);
            _dbContext.UserChallenges.Add(new UserChallenges { Challenge = challenge, User = user });
            _dbContext.SaveChanges();

            var controller = new StatisticsController(_dbContext, _envMock.Object, _loggerMock.Object);
            var result = controller.GetTopCompletedChallenges() as OkObjectResult;

            var value = result!.Value!;
            var challengesProperty = value.GetType().GetProperty("challenges");
            var challenges = challengesProperty!.GetValue(value) as IEnumerable<object>;

            Assert.That(result, Is.Not.Null);
            Assert.That(challenges, Is.Not.Null);
            Assert.That(challenges!.Count(), Is.EqualTo(1));
        }

        [Test]
        public void GetTopUsersMostPoints_ReturnsOrderedList()
        {
            _dbContext.Users.RemoveRange(_dbContext.Users);
            _dbContext.SaveChanges();

            _dbContext.Users.AddRange(
                new User { Username = "User1", Email = "testuser1@example.com", Points = 10 },
                new User { Username = "User2", Email = "testuser2@example.com", Points = 20 }
            );
            _dbContext.SaveChanges();

            var controller = new StatisticsController(_dbContext, _envMock.Object, _loggerMock.Object);
            var result = controller.GetTopUsersMostPoints() as OkObjectResult;

            var value = result!.Value!;
            var usersProperty = value.GetType().GetProperty("users");
            var users = usersProperty!.GetValue(value) as IEnumerable<object>;

            Assert.That(result, Is.Not.Null);
            Assert.That(users, Is.Not.Null);
            Assert.That(users!.Count(), Is.EqualTo(2));
        }

        [Test]
        public void GetMonthlyActiveUsers_ReturnsUsersForMonth()
        {
            _dbContext.Users.RemoveRange(_dbContext.Users);
            _dbContext.SaveChanges();

            var now = DateTime.Now;
            _dbContext.Users.Add(new User { Username = "ActiveUser", Email = "activeuser@example.com", LastLogin = now });
            _dbContext.SaveChanges();

            var controller = new StatisticsController(_dbContext, _envMock.Object, _loggerMock.Object);
            var result = controller.GetMonthlyActiveUsers(now.Year, now.Month) as OkObjectResult;

            var value = result!.Value!;
            var loginsProperty = value.GetType().GetProperty("logins");
            var logins = loginsProperty!.GetValue(value) as IEnumerable<object>;

            Assert.That(result, Is.Not.Null);
            Assert.That(logins, Is.Not.Null);
            Assert.That(logins!.Count(), Is.EqualTo(1));
        }
    }
}
