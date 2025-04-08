using EcoChallenger.Controllers;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace EcoChallengerTest.UnitTest
{
    public class RegisterControllerTest
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "EcoChallengerDB")
            .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();

            return context;
        }

        [Test]
        public async Task RegisterAccount_Success()
        {
            //Arrange
            var context = GetInMemoryDbContext();
            var controller = new RegisterController(context);
            var newUser = new User { Email = "test@gmail.com",
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("123")
            };

            //Act
            var result = await controller.RegisterAccount(newUser) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successValue, Is.Not.Null);
            Assert.That(successValue, Is.True);
        }

        [Test]
        public async Task RegisterAccount_EmailAlreadyExists()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var existingUser = new User { 
                Email = "test@gmail.com", 
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("123")
            };

            await context.Users.AddAsync(existingUser);
            await context.SaveChangesAsync();

            var controller = new RegisterController(context);
            var newUser = new User { Email = "test@gmail.com",
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("123") };

            // Act
            var result = await controller.RegisterAccount(newUser);
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.False);
        }

        [Test]
        public async Task RegisterAccount_ThrowsException()
        {
            var failingContext = GetInMemoryDbContext();
            failingContext.Database.EnsureCreated();
            failingContext.Dispose(); // Simulate a failure scenario (DB is closed)

            var controller = new RegisterController(failingContext);
            var newUser = new User { 
                Email = "test@gmail.com",
                Username = "test", 
                Password = PasswordGenerator.GeneratePasswordHash("123") 
            };

            // Act
            var result = await controller.RegisterAccount(newUser);
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.False);
        }
    }
}