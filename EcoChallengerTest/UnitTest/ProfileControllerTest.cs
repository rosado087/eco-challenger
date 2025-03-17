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
using System.Collections.Generic;


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
            var result = await _controller.UserList(new ProfileFriendModel { Id = testUser.Id, FriendUsername = testUser2.Username});
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
            var result = await _controller.AddFriend(new ProfileFriendModel { Id = testUser.Id, FriendUsername = testUser2.Username }) as JsonResult;

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
            var result = await _controller.GetFriends(testUser.Id) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var valueProperty = result.Value.GetType().GetProperty("friends")?.GetValue(result.Value);
            var friendsList = valueProperty as IEnumerable<object>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.True(successValue);
            Assert.NotNull(friendsList);
            Assert.Single(friendsList);

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
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.True(successValue);

            var valueProperty = result.Value.GetType().GetProperty("friends").GetValue(result.Value);
            var friendsList = valueProperty as IEnumerable<object>;
            Assert.NotNull(friendsList);
            Assert.Empty(friendsList);
        }

        [Fact]
        public async Task GetFriends_Returns_NotFound_For_Nonexistent_User()
        {
            // Act
            var result = await _controller.GetFriends(9999999) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);

            var messageProperty = result.Value.GetType().GetProperty("message").GetValue(result.Value);
            Assert.Equal("Utilizador não encontrado.", messageProperty);
        }
        
        [Fact]
        public async Task GetUserInfo_Returns_User_Info_When_User_Exists()
        {
            // Arrange
            var testUser = new User
            {
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
            var result = await _controller.GetUserInfo(testUser.Id) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var usernameProperty = result.Value.GetType().GetProperty("username");
            var usernameValue = (string)usernameProperty.GetValue(result.Value);

            var emailProperty = result.Value.GetType().GetProperty("email");
            var emailValue = (string)emailProperty.GetValue(result.Value);

            var pointsProperty = result.Value.GetType().GetProperty("points");
            var pointsValue = (int)pointsProperty.GetValue(result.Value);

            var tagProperty = result.Value.GetType().GetProperty("tag");
            var tagValue = (string)tagProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.True(successValue);
            Assert.Equal("testUser", usernameValue);
            Assert.Equal("test@example.com", emailValue);
            Assert.Equal(100, pointsValue);
            Assert.Equal("EcoWarrior", tagValue);
        }

        [Fact]
        public async Task GetUserInfo_Returns_Error_When_User_Does_Not_Exist()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act
            var result = await _controller.GetUserInfo(nonExistentUserId) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("O utilizador não existe", messageValue);
        }

        [Fact]
        public async Task GetUserInfo_Returns_Error_When_Exception_Occurs()
        {
            // Arrange
            var userId = 1;

            _dbContext.Dispose();

            // Act
            var result = await _controller.GetUserInfo(userId) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.NotNull(messageValue);
        }

        [Fact]
        public async Task EditUserInfo_Successfully_Updates_User()
        {
            // Arrange
            var user = new User {Username = "OldName", Email = "user@example.com", Points = 10 };
            _dbContext.Users.Add(user);

            var tag = new Tag {Name = "Eco-Warrior"};
            _dbContext.Tags.Add(tag);
            
            var tu = new TagUsers {User = user, Tag = tag, SelectedTag = false};
            _dbContext.TagUsers.Add(tu);

            await _dbContext.SaveChangesAsync();
            
            var profileEdit = new ProfileEditModel { Id = user.Id, Username = "NewName", Tag = "Eco-Warrior" };

            // Act
            var result = await _controller.EditUserInfo(profileEdit) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var usernameProperty = result.Value.GetType().GetProperty("username");
            var usernameValue = (string)usernameProperty.GetValue(result.Value);
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.True(successValue);
            Assert.Equal("NewName", usernameValue);
        }

        [Fact]
        public async Task EditUserInfo_Returns_Error_When_User_Not_Found()
        {
            // Arrange
            var profileEdit = new ProfileEditModel { Id = 999, Username = "NewName", Tag = "EcoWarrior" };

            // Act
            var result = await _controller.EditUserInfo(profileEdit) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("O utilizador não existe", messageValue);
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
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("Username é inválido", messageValue);
        }

        [Fact]
        public async Task EditUserInfo_Returns_Error_On_Exception()
        {
            // Arrange
            var profileEdit = new ProfileEditModel { Username = "NewName", Tag = "EcoWarrior" };

            
            _dbContext.Dispose();

            // Act
            var result = await _controller.EditUserInfo(profileEdit) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("Ocorreu um erro ao atualizar os seus dados", messageValue);
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
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var listProperty = result.Value.GetType().GetProperty("list")?.GetValue(result.Value);
            var tagList = listProperty as IEnumerable<object>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.True(successValue);
            Assert.NotNull(tagList);
            Assert.Equal(2, tagList.Count()); // Deve conter 2 tags
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
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var listProperty = result.Value.GetType().GetProperty("list")?.GetValue(result.Value);
            var tagList = listProperty as IEnumerable<object>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.True(successValue);
            Assert.NotNull(tagList);
            Assert.Empty(tagList);
        }

        [Fact]
        public async Task GetTags_Returns_Error_When_User_Not_Found()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act
            var result = await _controller.GetTags(nonExistentUserId) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("O utilizador não existe", messageValue);
        }

        [Fact]
        public async Task GetTags_Returns_Error_On_Exception()
        {
            // Arrange
            var userId = 1;

            _dbContext.Dispose();

            // Act
            var result = await _controller.GetTags(userId) as JsonResult;
            var successProperty = result.Value.GetType().GetProperty("success");
            var successValue = (bool)successProperty.GetValue(result.Value);

            var messageProperty = result.Value.GetType().GetProperty("message");
            var messageValue = (string)messageProperty.GetValue(result.Value);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(successProperty);
            Assert.False(successValue);
            Assert.Equal("Não foi possível encontrar as suas tags", messageValue);
        }

    }
}
