using EcoChallenger.Controllers;
using EcoChallenger.Models;
using EcoChallenger.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoChallengerTest.UnitTest
{
    public class GamificationTest
    {

        private readonly AppDbContext _dbContext;
        private readonly Mock<IConfiguration> _config;
        

        public GamificationTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
             _dbContext = new AppDbContext(options);
             _config = new Mock<IConfiguration>();


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
        public async Task Rotate_Daily_Challenges_Successfully()
        {
            

            var mockLogger = new Mock<ILogger<DailyTaskService>>();
            var serviceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock.Setup(x => x.GetService(typeof (AppDbContext)))
                .Returns(_dbContext);


            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();
            serviceScopeMock.Setup(x => x.ServiceProvider).Returns(serviceProviderMock.Object);
            serviceScopeFactoryMock.Setup(x => x.CreateScope()).Returns(serviceScopeMock.Object);

            var service = new DailyTaskService(serviceScopeFactoryMock.Object, mockLogger.Object);
            var cts = new CancellationTokenSource();
            
            cts.CancelAfter(3000);
            var userChallenges = await _dbContext.UserChallenges.ToListAsync();

            //Verifica que não existem desafios atribuidos
            Assert.Equal(0, userChallenges.Count);

            await service.StartAsync(cts.Token);

            userChallenges = await _dbContext.UserChallenges.ToListAsync();
            //Verifica que foram atribuidos 3 desafio diarios
            Assert.Equal(3, userChallenges.Count);

            mockLogger.Verify(log => log.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v!.ToString().Contains("Rotação de desafios diários foram feitos com sucesso")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

            mockLogger.Verify(log => log.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v!.ToString().Contains("Próxima rotação de desafios diários é")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);

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
    }
}
