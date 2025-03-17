using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
using EcoChallenger.Controllers;
using EcoChallenger.Models;
using EcoChallenger.Utils;
using Microsoft.Extensions.Logging;

namespace EcoChallengerTest.UnitTest
{

    public class ProfileControllerTest
    {


        private readonly ProfileController _controller;
        private readonly AppDbContext _dbContext;
        private readonly Mock<IConfiguration> _config;
        private readonly Mock<ILogger<ProfileController>> _mockLogger;

        public ProfileControllerTest()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new AppDbContext(options);
            _config = new Mock<IConfiguration>();
            _mockLogger = new Mock<ILogger<ProfileController>>();

            _controller = new ProfileController(_dbContext, _mockLogger.Object);
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

            var valueProperty = result.Value.GetType().GetProperty("success");
            Assert.True((bool)valueProperty.GetValue(result.Value));
        }

        [Fact]
        public async Task GetFriends_Returns_List_Of_Friends()
        {
            // Arrange
            var testUser = new User
            {
                Email = "test@example.com",
                Username = "test",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            var friendUser = new User
            {
                Email = "friend@example.com",
                Username = "friendUser",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            _dbContext.Users.Add(testUser);
            _dbContext.Users.Add(friendUser);
            await _dbContext.SaveChangesAsync();

            // Add Friendship
            _dbContext.Friendships.Add(new Friend { UserId = testUser.Id, FriendId = friendUser.Id });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetFriends(testUser.Id) as JsonResult; // da erro aqui

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var valueProperty = result.Value.GetType().GetProperty("friends").GetValue(result.Value);
            Assert.NotNull(valueProperty);
            Assert.Single((ICollection<object>)valueProperty); // Should have one friend
        }

        [Fact]
        public async Task GetFriends_Returns_Empty_List_When_No_Friends()
        {
            // Arrange
            var testUser = new User
            {
                Email = "lonely@example.com",
                Username = "lonelyUser",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            _dbContext.Users.Add(testUser);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetFriends(testUser.Id) as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);

            var valueProperty = result.Value.GetType().GetProperty("friends").GetValue(result.Value);
            Assert.NotNull(valueProperty);
            Assert.Empty((ICollection<object>)valueProperty); // Should return an empty list
        }

        [Fact]
        public async Task GetFriends_Returns_NotFound_For_Nonexistent_User()
        {
            // Act
            var result = await _controller.GetFriends(9999999) as JsonResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);

            var messageProperty = result.Value.GetType().GetProperty("message").GetValue(result.Value);
            Assert.Equal("Utilizador não encontrado.", messageProperty);
        }

        [Fact]
        public async Task GetUserInfo_Returns_User_Info_When_User_Exists()
        {
            // Arrange
            var userId = 1;
            var testUser = new User
            {
                Id = userId,
                Username = "testUser",
                Email = "test@example.com",
                Points = 100
            };

            var testTag = new TagUsers
            {
                SelectedTag = true,
                User = testUser,
                Tag = new Tag { Name = "EcoWarrior" }
            };

            _dbContext.Users.Add(testUser);
            _dbContext.TagUsers.Add(testTag);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetUserInfo(userId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value as dynamic;
            Assert.True(value.success);
            Assert.Equal("testUser", value.username);
            Assert.Equal("test@example.com", value.email);
            Assert.Equal(100, value.points);
            Assert.Equal("EcoWarrior", value.tag);
        }

        [Fact]
        public async Task GetUserInfo_Returns_Error_When_User_Does_Not_Exist()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act
            var result = await _controller.GetUserInfo(nonExistentUserId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value as dynamic;
            Assert.False(value.success);
            Assert.Equal("O utilizador não existe", value.message);
        }

        [Fact]
        public async Task GetUserInfo_Returns_Error_When_Exception_Occurs()
        {
            // Arrange
            var userId = 1;

            // Fechar o contexto para simular erro de acesso ao banco de dados
            _dbContext.Dispose();

            // Act
            var result = await _controller.GetUserInfo(userId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value as dynamic;
            Assert.False(value.success);
            Assert.NotNull(value.message); // Deve conter uma mensagem de erro
        }

        [Fact]
        public async Task EditUserInfo_Successfully_Updates_User()
        {
            // Arrange
            var user = new User { Username = "OldName", Email = "user@example.com", Points = 10 };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profileEdit = new ProfileEditModel { Id = user.Id, Username = "NewName", Tag = "EcoWarrior" };

            // Act
            var result = await _controller.EditUserInfo(profileEdit) as JsonResult;
            var response = result?.Value as dynamic;

            // Assert
            Assert.NotNull(result);
            Assert.True(response.success);
            Assert.Equal("NewName", response.username);
        }

        [Fact]
        public async Task EditUserInfo_Returns_Error_When_User_Not_Found()
        {
            // Arrange
            var profileEdit = new ProfileEditModel { Id = 999, Username = "NewName", Tag = "EcoWarrior" };

            // Act
            var result = await _controller.EditUserInfo(profileEdit) as JsonResult;
            var response = result?.Value as dynamic;

            // Assert
            Assert.NotNull(result);
            Assert.False(response.success);
            Assert.Equal("O utilizador não existe", response.message);
        }

        [Fact]
        public async Task EditUserInfo_Returns_Error_When_Username_Is_Invalid()
        {
            // Arrange
            var user = new User { Username = "ValidUser", Email = "user@example.com", Points = 10 };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profileEdit = new ProfileEditModel { Id = user.Id, Username = "" }; // Nome inválido

            // Act
            var result = await _controller.EditUserInfo(profileEdit) as JsonResult;
            var response = result?.Value as dynamic;

            // Assert
            Assert.NotNull(result);
            Assert.False(response.success);
            Assert.Equal("Username é inválido", response.message);
        }

        [Fact]
        public async Task EditUserInfo_Returns_Success_When_Tag_Does_Not_Exist()
        {
            // Arrange
            var user = new User { Username = "User", Email = "user@example.com", Points = 10 };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profileEdit = new ProfileEditModel { Id = user.Id, Username = "UpdatedUser", Tag = "NonExistentTag" };

            // Act
            var result = await _controller.EditUserInfo(profileEdit) as JsonResult;
            var response = result?.Value as dynamic;

            // Assert
            Assert.NotNull(result);
            Assert.True(response.success);
            Assert.Equal("UpdatedUser", response.username);
        }

        [Fact]
        public async Task EditUserInfo_Returns_Error_On_Exception()
        {
            // Arrange
            var profileEdit = new ProfileEditModel { Id = 1, Username = "NewName", Tag = "EcoWarrior" };

            // Fechar o contexto para simular erro de acesso ao banco de dados
            _dbContext.Dispose();

            // Act
            var result = await _controller.EditUserInfo(profileEdit) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value as dynamic;
            Assert.False(value.success);
            Assert.Equal("Ocorreu um erro ao atualizar os seus dados", value.message);
        }

        [Fact]
        public async Task GetTags_Returns_List_When_User_Has_Tags()
        {
            // Arrange
            var user = new User { Username = "UserWithTags", Email = "user@tags.com" };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var tag1 = new Tag { Name = "EcoWarrior" };
            var tag2 = new Tag { Name = "Recycler" };
            _dbContext.Tags.AddRange(tag1, tag2);
            await _dbContext.SaveChangesAsync();

            _dbContext.TagUsers.AddRange(
                new TagUsers { User = user, Tag = tag1 },
                new TagUsers { User = user, Tag = tag2 }
            );
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetTags(user.Id) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value as dynamic;
            Assert.True(value.success);
            Assert.NotNull(value.list);
            Assert.Equal(2, ((ICollection<object>)value.list).Count); // Deve conter 2 tags
        }

        [Fact]
        public async Task GetTags_Returns_Empty_List_When_User_Has_No_Tags()
        {
            // Arrange
            var user = new User { Username = "UserNoTags", Email = "usernotags@tags.com" };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller.GetTags(user.Id) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value as dynamic;
            Assert.True(value.success);
            Assert.NotNull(value.list);
            Assert.Empty((ICollection<object>)value.list); // Deve retornar lista vazia
        }

        [Fact]
        public async Task GetTags_Returns_Error_When_User_Not_Found()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act
            var result = await _controller.GetTags(nonExistentUserId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value as dynamic;
            Assert.False(value.success);
            Assert.Equal("O utilizador não existe", value.message);
        }

        [Fact]
        public async Task GetTags_Returns_Error_On_Exception()
        {
            // Arrange
            var userId = 1;

            // Simular erro fechando o contexto
            _dbContext.Dispose();

            // Act
            var result = await _controller.GetTags(userId) as JsonResult;

            // Assert
            Assert.NotNull(result);
            var value = result.Value as dynamic;
            Assert.False(value.success);
            Assert.Equal("Não foi possível encontrar as suas tags", value.message);
        }


    }
}
