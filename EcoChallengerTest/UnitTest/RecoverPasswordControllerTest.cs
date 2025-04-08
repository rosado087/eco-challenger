using System.Collections.Generic;
using System.Threading.Tasks;
using EcoChallenger.Controllers;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace EcoChallengerTest.UnitTest
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

        [Test]
        public async Task SendRecoveryEmail_UserExists_EmailSent()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var mockEmailService = new Mock<IEmailService>();
            var mockLogger = new Mock<ILogger<RecoverPasswordController>>();

            List<KeyValuePair<string, string?>> inMemorySettings = new List<KeyValuePair<string, string?>>()
            {
                new KeyValuePair<string, string?>("ApplicationSettings:FrontEndUrl", "http://localhost")
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
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successValue, Is.True);
            mockEmailService.Verify(e => e.SendRecoveryEmailAsync(It.IsAny<string>(), It.IsAny<string>()),Times.Once);
        }


        [Test]
        public void CheckToken_ValidToken_ReturnsSuccess()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var validToken = TokenManager.CreateRecoveryUserToken(new User { Email = "test@gmail.com", Username = "test", Password = "123" });
            context.UserTokens.Add(validToken);
            context.SaveChanges();

            var controller = new RecoverPasswordController(context, null, null, null);

            // Act
            var result = controller.CheckToken(validToken.Token);
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successValue, Is.True);
        }

        [Test]
        public async Task SetNewPassword_ValidToken_UpdatesPassword()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            var user = new User { Email = "test@gmail.com", Username = "test", Password = "oldPassword" };
            var validToken = TokenManager.CreateRecoveryUserToken(user);
            context.Users.Add(user);
            context.UserTokens.Add(validToken);
            await context.SaveChangesAsync();

            var controller = new RecoverPasswordController(context, null, null, null);
            var request = new SetNewPasswordModel { Token = validToken.Token, Password = "newPassword" };

            // Act
            var result = await controller.SetNewPassword(request);
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successValue, Is.True);
            Assert.That(PasswordGenerator.ComparePasswordWithHash("newPassword", user.Password) , Is.True);
        }
    }
}
