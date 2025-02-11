using EcoChallenger.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        [Fact]
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
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            //Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.True(successValue);
        }

        [Fact]
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
            var result = await controller.RegisterAccount(newUser) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var messageProperty = result.Value.GetType().GetProperty("message");
            var successValue = (bool)successProperty.GetValue(result.Value);
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("Este email já existe", messageValue);
        }

        [Fact]
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
            var result = await controller.RegisterAccount(newUser) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var messageProperty = result.Value.GetType().GetProperty("message");
            var successValue = (bool)successProperty.GetValue(result.Value);
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.NotNull(messageProperty);
            Assert.False(successValue);
            Assert.Contains("Cannot access a disposed context instance", messageValue);
        }
    }
}