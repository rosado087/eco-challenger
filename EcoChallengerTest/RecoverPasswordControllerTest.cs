using EcoChallenger.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace EcoChallengerTest
{
    public class RecoverPasswordControllerTest
    {
        private AppDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "RecoverPasswordDB")
                .Options;

            var context = new AppDbContext(options);
            context.Database.EnsureCreated();

            return context;
        }

        [Fact]
        public async Task SendRecoveryEmail_UserExists_EmailSent()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailService = new Mock<IEmailService>();
            var mockLogger = new Mock<ILogger<RecoverPasswordController>>();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "ApplicationSettings:FrontEndUrl", "http://localhost" }
            };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            var user = new User { Email = "test@gmail.com", Username = "test", Password = "123" };
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            var controller = new RecoverPasswordController(context, mockEmailService.Object, configuration, mockLogger.Object);
            var request = new SendRecoveryEmailModel { Email = "test@gmail.com" };

            // Act
            var result = await controller.SendRecoveryEmail(request) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.True(successValue);
            mockEmailService.Verify(e => e.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }


        [Fact]
        public void CheckToken_ValidToken_ReturnsSuccess()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var validToken = TokenManager.CreateUserToken(new User { Email = "test@gmail.com", Username = "test" , Password = "123"});
            context.UserTokens.Add(validToken);
            context.SaveChanges();

            var controller = new RecoverPasswordController(context, null, null, null);

            // Act
            var result = controller.CheckToken(validToken.Token) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.True(successValue);
        }

        [Fact]
        public async Task SetNewPassword_ValidToken_UpdatesPassword()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var user = new User { Email = "test@gmail.com", Username = "test", Password = "oldPassword" };
            var validToken = TokenManager.CreateUserToken(user);
            context.Users.Add(user);
            context.UserTokens.Add(validToken);
            await context.SaveChangesAsync();

            var controller = new RecoverPasswordController(context, null, null, null);
            var request = new SetNewPasswordModel { Token = validToken.Token, Password = "newPassword" };

            // Act
            var result = await controller.SetNewPassword(request) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.True(successValue);
            Assert.NotEqual("oldPassword", user.Password);
        }
    }
}
