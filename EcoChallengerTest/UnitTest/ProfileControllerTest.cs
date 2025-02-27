using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using EcoChallenger.Controllers;

namespace EcoChallengerTest.UnitTest
{
   
    public class ProfileControllerTest
    {


        private readonly ProfileController _controller;
        private readonly AppDbContext _dbContext;
        private readonly Mock<IConfiguration> _config;

        public ProfileControllerTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new AppDbContext(options);
            _config = new Mock<IConfiguration>();

            _controller = new ProfileController(_dbContext);
        }

        [Fact]
        public async Task Show_Possible_Friends_When_Searching_To_Add_Friend()
        {
            // Arrange
            var testUser = new User
            {
                Email = "test@example.com",
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            var testUser2 = new User
            {
                Email = "test2@example.com",
                Username = "test2",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            _dbContext.Users.Add(testUser);
            _dbContext.Users.Add(testUser2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.UserList([testUser.Username, testUser2.Username]);
            Assert.NotNull(result);
            Assert.NotNull(result.Value);
            var valueProperty = result.Value.GetType().GetProperty("usernames").GetValue(result.Value);
            Assert.Equal(1, ((ICollection<string>) valueProperty).Count);
        }

        [Fact]
        public async Task Add_Friend_Successfully()
        {
            // Arrange
            var testUser = new User
            {
                Email = "test@example.com",
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            var testUser2 = new User
            {
                Email = "test2@example.com",
                Username = "test2",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            _dbContext.Users.Add(testUser);
            _dbContext.Users.Add(testUser2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.AddFriend([testUser.Username, testUser2.Username]) as JsonResult;

            var valueProperty = result.Value.GetType().GetProperty("result");
            Assert.True((bool)valueProperty.GetValue(result.Value));
        }

    }
}
