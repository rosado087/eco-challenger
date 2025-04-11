using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using EcoChallenger.Controllers;
using EcoChallenger.Models;
using EcoChallenger.Utils;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;


namespace EcoChallengerTest.UnitTest
{

    public class ProfileControllerTest
    {
        private ProfileController? _controller;
        private AppDbContext? _dbContext;
        private Mock<ILogger<ProfileController>>? _mockLogger;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _dbContext = new AppDbContext(options);
            _mockLogger = new Mock<ILogger<ProfileController>>();

            _controller = new ProfileController(_dbContext, _mockLogger.Object);
        }

        [Test]
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

            _dbContext!.Users.Add(testUser);
            _dbContext.Users.Add(testUser2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller!.UserList(new ProfileFriendModel { Id = testUser.Id, FriendUsername = testUser2.Username});
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.Not.Null);
            
            var valueProperty = result.Value?.GetType().GetProperty("usernames")?.GetValue(result.Value);
            Assert.That(valueProperty, Is.Not.Null);
            Assert.That(((ICollection<string>) valueProperty!).Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
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

            _dbContext!.Users.Add(testUser);
            _dbContext.Users.Add(testUser2);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller!.AddFriend(new ProfileFriendModel { Id = testUser.Id, FriendUsername = testUser2.Username }) as JsonResult;

            var valueProperty = result.Value?.GetType().GetProperty("success");
            Assert.That(valueProperty?.GetValue(result.Value), Is.True);
        }

        [Test]
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

            _dbContext!.Users.Add(testUser);
            _dbContext.Users.Add(friendUser);
            await _dbContext.SaveChangesAsync();

            // Add Friendship
            _dbContext.Friendships.Add(new Friend { UserId = testUser.Id, FriendId = friendUser.Id });
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller!.GetFriends(testUser.Id) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            var valueProperty = result.Value?.GetType().GetProperty("friends")?.GetValue(result.Value);
            var friendsList = valueProperty as IEnumerable<object>;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.True);
            Assert.That(friendsList, Is.Not.Null);
            Assert.That(friendsList!.Count(), Is.EqualTo(1));
        }

        [Test]
        public async Task GetFriends_Returns_Empty_List_When_No_Friends()
        {
            // Arrange
            var testUser = new User
            {
                Email = "lonely@example.com",
                Username = "lonelyUser",
                Password = PasswordGenerator.GeneratePasswordHash("correctPassword")
            };

            _dbContext!.Users.Add(testUser);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller!.GetFriends(testUser.Id) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.True);

            var valueProperty = result.Value?.GetType().GetProperty("friends")?.GetValue(result.Value);
            var friendsList = valueProperty as IEnumerable<object>;

            Assert.That(friendsList, Is.Not.Null);
            Assert.That(friendsList, Is.Empty);
        }

        [Test]
        public async Task GetFriends_Returns_NotFound_For_Nonexistent_User()
        {
            // Act
            var result = await _controller!.GetFriends(9999999) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.False);
        }
        
        [Test]
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
                Tag = new Tag { Name = "EcoWarrior",
                    Price = 10,
                    BackgroundColor = "#355735",
                    TextColor = "#FFFFFF"
                }
            };

            _dbContext!.Users.Add(testUser);
            _dbContext.TagUsers.Add(testTag);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _controller!.GetUserInfo(testUser.Id) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            var usernameProperty = result.Value?.GetType().GetProperty("username");
            var usernameValue = usernameProperty?.GetValue(result.Value);

            var emailProperty = result.Value?.GetType().GetProperty("email");
            var emailValue = emailProperty?.GetValue(result.Value);

            var pointsProperty = result.Value?.GetType().GetProperty("points");
            var pointsValue = pointsProperty?.GetValue(result.Value);

            var tagProperty = result.Value?.GetType().GetProperty("tag");
            var tagValue = tagProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.True);

            Assert.That(usernameValue, Is.EqualTo("testUser"));
            Assert.That(emailValue, Is.EqualTo("test@example.com"));
            Assert.That(pointsValue, Is.EqualTo(100));
            Assert.That(tagValue, Is.EqualTo("EcoWarrior"));
        }

        [Test]
        public async Task GetUserInfo_Returns_Error_When_User_Does_Not_Exist()
        {
            // Arrange
            var nonExistentUserId = 999;

            // Act
            var result = await _controller!.GetUserInfo(nonExistentUserId) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.False);
        }

        [Test]
        public async Task GetUserInfo_Returns_Error_When_Exception_Occurs()
        {
            // Arrange
            var userId = 1;

            _dbContext!.Dispose();

            // Act
            var result = await _controller!.GetUserInfo(userId) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            var messageProperty = result.Value?.GetType().GetProperty("message");
            var messageValue = messageProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.False);
            Assert.That(messageValue, Is.Not.Null);
        }

        [Test]
        public async Task EditUserInfo_Successfully_Updates_User()
        {
            // Arrange
            var user = new User {Username = "OldName", Email = "user@example.com", Points = 10 };
            _dbContext!.Users.Add(user);

            var tag = new Tag {Name = "Eco-Warrior", Price = 10, BackgroundColor = "#355735", TextColor = "#FFFFFF" };
            _dbContext.Tags.Add(tag);
            
            var tu = new TagUsers {User = user, Tag = tag, SelectedTag = false};
            _dbContext.TagUsers.Add(tu);

            await _dbContext.SaveChangesAsync();
            
            var profileEdit = new ProfileEditModel { Id = user.Id, Username = "NewName" };

            // Act
            var result = await _controller!.EditUserInfo(profileEdit) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            var usernameProperty = result.Value?.GetType().GetProperty("username");
            var usernameValue = usernameProperty?.GetValue(result.Value);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.True);
            Assert.That(usernameValue, Is.EqualTo("NewName"));
        }

        [Test]
        public async Task EditUserInfo_Returns_Error_When_User_Not_Found()
        {
            // Arrange
            var profileEdit = new ProfileEditModel { Id = 999, Username = "NewName" };

            // Act
            var result = await _controller!.EditUserInfo(profileEdit) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.False);
        }

        [Test]
        public async Task EditUserInfo_Returns_Error_When_Username_Is_Invalid()
        {
            // Arrange
            var user = new User { Username = "ValidUser", Email = "user@example.com", Points = 10 };
            _dbContext!.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            var profileEdit = new ProfileEditModel { Id = user.Id, Username = "" }; // Nome inválido

            // Act
            var result = await _controller!.EditUserInfo(profileEdit) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.False);
        }

        [Test]
        public async Task EditUserInfo_Returns_Error_On_Exception()
        {
            // Arrange
            var profileEdit = new ProfileEditModel { Username = "NewName" };
            
            _dbContext!.Dispose();

            // Act
            var result = await _controller!.EditUserInfo(profileEdit) as JsonResult;
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(successProperty, Is.Not.Null);
            Assert.That(successValue, Is.False);
        }

        [Test]
        public async Task RemoveFriend_InvalidParameters_ReturnsError()
        {
            // Arrange
            var request = new ProfileFriendModel { Id = 0, FriendUsername = "" };

            // Act
            var result = await _controller!.RemoveFriend(request) as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            Assert.That(successValue, Is.False);
        }

        [Test]
        public async Task RemoveFriend_UserOrFriendNotFound_ReturnsError()
        {
            // Arrange
            var request = new ProfileFriendModel { Id = 99, FriendUsername = "NonExistentUser" };

            // Act
            var result = await _controller!.RemoveFriend(request) as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            Assert.That(successValue, Is.False);
        }

        [Test]
        public async Task RemoveFriend_FriendshipNotFound_ReturnsError()
        {
            // Arrange
            var user = new User { Username = "User1", Email = "user1@example.com" };
            var friend = new User { Username = "User2", Email = "user2@example.com" };

            _dbContext!.Users.Add(user);
            _dbContext.Users.Add(friend);
            await _dbContext.SaveChangesAsync();

            var request = new ProfileFriendModel { Id = user.Id, FriendUsername = friend.Username };

            // Act
            var result = await _controller!.RemoveFriend(request) as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            Assert.That(successValue, Is.False);
        }
        [Test]
        public async Task RemoveFriend_SuccessfullyRemovesFriendship()
        {
            // Arrange
            var user = new User { Username = "User158", Email = "user158@example.com" };
            var friend = new User { Username = "User258", Email = "user258@example.com" };
            _dbContext!.Users.Add(user);
            _dbContext.Users.Add(friend);
            await _dbContext.SaveChangesAsync();
            
            var friendship = new Friend { UserId = user.Id, FriendId = friend.Id };

            
            _dbContext.Friendships.Add(friendship);
            await _dbContext.SaveChangesAsync();

            var request = new ProfileFriendModel { Id = user.Id, FriendUsername = friend.Username };

            // Act
            var result = await _controller!.RemoveFriend(request) as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            var successProperty = result.Value?.GetType().GetProperty("success");
            var successValue = successProperty?.GetValue(result.Value);

            Assert.That(successValue, Is.True);
        }
    }
}
