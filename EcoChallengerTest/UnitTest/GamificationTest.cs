using EcoChallenger.Controllers;
using EcoChallenger.Models;
using EcoChallenger.Services;
using EcoChallenger.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EcoChallengerTest.UnitTest
{
    public class GamificationTest
    {

        private readonly AppDbContext _dbContext;
        private readonly GamificationController _controller;
        private readonly Mock<IConfiguration> _config;
        private readonly Mock<ILogger<GamificationController>> _mockLogger;
        

        public GamificationTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new AppDbContext(options);
            _config = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<GamificationController>>();
            _controller = new GamificationController(_dbContext, _mockLogger.Object);


            var user = new User { Username = "testuser", Email = "test@example.com", IsAdmin = false };
            _dbContext.Users.Add(user);
            _dbContext.Challenges.AddRange(
            new Challenge()
            {
                Title = "Daily Challenge 1",
                Description = "Description 1",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Title = "Daily Challenge 2",
                Description = "Description 2",
                Points = 10,
                Type = "Daily"
            }
            ,
            new Challenge()
            {
                Title = "Daily Challenge 3",
                Description = "Description 3",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Title = "Daily Challenge 4",
                Description = "Description 4",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Title = "Daily Challenge 5",
                Description = "Description 5",
                Points = 10,
                Type = "Daily"
            },
            new Challenge()
            {
                Title = "Weekly Challenge 1",
                Description = "Description 1",
                Points = 100,
                MaxProgress = 5,
                Type = "Weekly"
            },
            new Challenge()
            {
                Title = "Weekly Challenge 2",
                Description = "Description 2",
                Points = 160,
                MaxProgress = 7,
                Type = "Weekly"
            },
            new Challenge()
            {
                Title = "Weekly Challenge 3",
                Description = "Description 3",
                Points = 60,
                MaxProgress = 3,
                Type = "Weekly"
            }
            );

            _dbContext.SaveChangesAsync();

        }

        [Fact]
        public async void Rotate_Daily_Challenges_Successfully()
        {            
            var userChallenges = _dbContext.UserChallenges.ToList();

            //Verifica que não existem desafios atribuidos
            Assert.Empty(userChallenges);

            // User gamification rotation service to attribute 3 challenges
            var mockLogger = new Mock<ILogger<DailyTaskService>>(); // Setup logger mock interface

            // This IServiceScope is used do the se
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock.Setup(x => x.GetService(typeof (AppDbContext))) // Make sure DB is running
                .Returns(_dbContext);

            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
            serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);
            
            var service = new DailyTaskService(serviceScopeFactoryMock.Object, mockLogger.Object);
            var cts = new CancellationTokenSource();
            
            // This will start the service for task attribution
            await service.StartAsync(cts.Token);

            Thread.Sleep(3000); // Make sure rotation is done

            userChallenges = _dbContext.UserChallenges.ToList();
            //Verifica que foram atribuidos 3 desafio diarios
            Assert.Equal(3, userChallenges.Count);

            // Make sure the service is not running anymore
            await service.StopAsync(cts.Token);
        }

        [Fact]
        public async Task Rotate_Weekly_Challenges_Successfully()
        {
            var mockLogger = new Mock<ILogger<WeeklyTaskService>>();

            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(x => x.GetService(typeof(AppDbContext)))
                .Returns(_dbContext);


            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
            serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);

            var service = new WeeklyTaskService(serviceScopeFactoryMock.Object, mockLogger.Object);
            var cts = new CancellationTokenSource();
            cts.CancelAfter(2000);

            

            var userChallenges = await _dbContext.UserChallenges.ToListAsync();

            //Verifica que não existem desafios atribuidos
            Assert.Equal(0, userChallenges.Count);

            await service.StartAsync(cts.Token);

            userChallenges = await _dbContext.UserChallenges.ToListAsync();
            //Verifica que foram atribuidos 2 desafios semanais
            Assert.Equal(2, userChallenges.Count);

            mockLogger.Verify(log => log.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v!.ToString().Contains("Rotação de desafios semanais foram feitos com sucesso")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            mockLogger.Verify(log => log.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v!.ToString().Contains("Próxima rotação de desafios semanais é")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
            
        }

        [Fact]
        public async Task CompleteChallenge_Works_AsExpected()
        {          
            // Arrange

            var testUser = new User
            {
                Email = "test@example.com",
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            var challenge1 = new Challenge
            {
                Title = "New Challenge",
                Description = "Challenge 1",
                Points = 10,
                Type = "Daily",
                MaxProgress = 1
            };

            var userChallenge1 = new UserChallenges
            {
                User = testUser,
                Challenge = challenge1,
                WasConcluded = false,
                Progress = 0
            };

            _dbContext.Users.Add(testUser);
            _dbContext.Challenges.Add(challenge1);
            _dbContext.UserChallenges.Add(userChallenge1);
            await _dbContext.SaveChangesAsync();


            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("userid", testUser.Id.ToString())
            }));

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            UserContext.Initialize(httpContextAccessor.Object);

            // Act 
            var result = await _controller.CompleteChallenge(challenge1.Id) as JsonResult;

            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);
            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.True(successValue);
            Assert.Equal("Desafio concluido com sucesso.", messageValue);
        }


        [Fact]
        public async Task CompleteChallenge_Already_Completed()
        {          
            // Arrange
            var testUser = new User
            {
                Email = "test@example.com",
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            var challenge1 = new Challenge
            {
                Title = "New Challenge",
                Description = "Challenge 1",
                Points = 10,
                Type = "Daily",
                MaxProgress = 1
            };

            var userChallenge1 = new UserChallenges
            {
                User = testUser,
                Challenge = challenge1,
                WasConcluded = true,
                Progress = 0
            };

            _dbContext.Users.Add(testUser);
            _dbContext.Challenges.Add(challenge1);
            _dbContext.UserChallenges.Add(userChallenge1);
            await _dbContext.SaveChangesAsync();


            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("userid", testUser.Id.ToString())
            }));

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            UserContext.Initialize(httpContextAccessor.Object);

            // Act 
            var result = await _controller.CompleteChallenge(challenge1.Id) as JsonResult;

            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);
            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("O desafio já está concluído.", messageValue);
        }

        [Fact]
        public async Task CompleteChallenge_Does_Not_Exist()
        {          
            // Arrange

            int i = 999;

            // Act 
            var result = await _controller.CompleteChallenge(i) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);
            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("O desafio não existe.", messageValue);
        }

        [Fact]
        public async Task AddProgress_Returns_Error_When_Challenge_NotFound()
        {
            // Arrange
            int i = 999;

            // Act
            var result = await _controller.AddProgress(i) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);
            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("O desafio não existe.", messageValue);
        }

        [Fact]
        public async Task AddProgress_Returns_Error_When_Challenge_Already_Completed()
        {
            // Arrange
            var testUser = new User
            {
                Email = "test@example.com",
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            var challenge1 = new Challenge
            {
                Title = "New Challenge",
                Description = "Challenge 1",
                Points = 40,
                Type = "Weekly",
                MaxProgress = 1
            };

            var userChallenge1 = new UserChallenges
            {
                User = testUser,
                Challenge = challenge1,
                WasConcluded = true,
                Progress = 0
            };

            _dbContext.Users.Add(testUser);
            _dbContext.Challenges.Add(challenge1);
            _dbContext.UserChallenges.Add(userChallenge1);
            await _dbContext.SaveChangesAsync();


            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("userid", testUser.Id.ToString())
            }));

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            UserContext.Initialize(httpContextAccessor.Object);

            // Act 
            var result = await _controller.AddProgress(challenge1.Id) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);
            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("O desafio já está concluído.", messageValue);
        }

        [Fact]
        public async Task AddProgress_Successfully()
        {
            // Arrange
            var testUser = new User
            {
                Email = "test@example.com",
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            var challenge1 = new Challenge
            {
                Title = "New Challenge",
                Description = "Challenge 1",
                Points = 40,
                Type = "Weekly",
                MaxProgress = 5
            };

            var userChallenge1 = new UserChallenges
            {
                User = testUser,
                Challenge = challenge1,
                WasConcluded = false,
                Progress = 0
            };

            _dbContext.Users.Add(testUser);
            _dbContext.Challenges.Add(challenge1);
            _dbContext.UserChallenges.Add(userChallenge1);
            await _dbContext.SaveChangesAsync();


            var httpContext = new DefaultHttpContext();
            httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("userid", testUser.Id.ToString())
            }));

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            UserContext.Initialize(httpContextAccessor.Object);

            // Act 
            var result = await _controller.AddProgress(challenge1.Id) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);
            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.True(successValue);
            Assert.Equal("Progresso adicionado com sucesso.", messageValue);
        }

    }
}
