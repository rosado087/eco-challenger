using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using EcoChallenger;
using EcoChallenger.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using EcoChallenger.Utils;
using EcoChallengerTest.Utils;

namespace EcoChallengerTest.IntegrationTest 
{
    public class ProfileControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory _factory;

        public ProfileControllerTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task GenerateToken_UserExists_ReturnsToken()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();

            var user = new User { Username = "testuser", Email = "test@example.com", IsAdmin = false };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var token = TokenManager.GenerateJWT(user);
            Assert.NotNull(token);

            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync("/api/Profile/GenerateToken");

            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode, $"API call failed: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(content), "Response body is empty");

            var result = JsonConvert.DeserializeObject<dynamic>(content);
            Assert.NotNull(result);

            Assert.True((bool)result.success, $"API returned failure: {content}");
            Assert.NotNull(result.token);

            await GenericFunctions.ResetDatabase();
        }

        [Fact]
        public async Task GetUserInfo_UserExists_ReturnsUserInfo()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Users.RemoveRange(context.Users);
            await context.SaveChangesAsync();

            var user = new User { Username = "testuser2", Email = "test2@example.com" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var savedUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "testuser2");
            Assert.NotNull(savedUser);
            Assert.True(savedUser.Id > 0, "User ID was not assigned");

            var token = TokenManager.GenerateJWT(savedUser);
            Assert.NotNull(token);

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/api/Profile/GetUserInfo/{savedUser.Id}");
            
            Assert.NotNull(response);
            Assert.True(response.IsSuccessStatusCode, $"API call failed: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            Assert.False(string.IsNullOrEmpty(content), "Response body is empty");

            var result = JsonConvert.DeserializeObject<dynamic>(content);
            Assert.NotNull(result);
            Assert.True((bool)result.success, $"API returned failure: {content}");
            Assert.Equal("testuser2", (string)result.username);

            await GenericFunctions.ResetDatabase();
        }

        [Fact]
        public async Task EditUserInfo_ValidRequest_UpdatesUser()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User { Username = "olduser", Email = "old@example.com" };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            var token = TokenManager.GenerateJWT(user);
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var updateProfile = new { Id = user.Id, Username = "newuser", Tag = "Eco" };
            var content = new StringContent(JsonConvert.SerializeObject(updateProfile), Encoding.UTF8, "application/json");
            
            var response = await _client.PostAsync("/api/Profile/EditUserInfo", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
            
            Assert.NotNull(result);
            Assert.True((bool)result.success, $"API returned failure: {responseBody}");
            Assert.Equal("newuser", (string)result.username);

            await GenericFunctions.ResetDatabase();
        }

        [Fact]
        public async Task GetTags_UserHasTags_ReturnsTags()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User { Username = "taggeduser", Email = "taggeduser@example.com" };
            context.Users.Add(user);
            context.TagUsers.Add(new TagUsers { User = user, Tag = new Tag { Name = "EcoWarrior" ,Price = 10,
                BackgroundColor = "#355735",
                TextColor = "#FFFFFF"
            }, SelectedTag = true });
            await context.SaveChangesAsync();

            var token = TokenManager.GenerateJWT(user);
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await _client.GetAsync($"/api/Profile/GetTags/{user.Id}");
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(content);

            Assert.NotNull(result);
            Assert.True((bool)result.success, $"API returned failure: {content}");
            Assert.Contains("EcoWarrior", result.list.ToObject<List<string>>());

            await GenericFunctions.ResetDatabase();
        }

        [Fact]
        public async Task AddFriend_ValidRequest_AddsFriend()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User { Username = "friend1", Email = "friend1@example.com" };
            var friend = new User { Username = "friend2", Email = "friend2@example.com" };
            context.Users.AddRange(user, friend);
            await context.SaveChangesAsync();

            var token = TokenManager.GenerateJWT(user);
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var friendRequest = new { Id = user.Id, FriendUsername = "friend2" };
            var content = new StringContent(JsonConvert.SerializeObject(friendRequest), Encoding.UTF8, "application/json");
            
            var response = await _client.PostAsync("/api/Profile/AddFriend", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
            
            Assert.NotNull(result);
            Assert.True((bool)result.success, $"API returned failure: {responseBody}");
            Assert.Equal("Amigo adicionado com sucesso!", (string)result.message);
            
            await GenericFunctions.ResetDatabase();
        }

        [Fact]
        public async Task RemoveFriend_ValidRequest_RemovesFriend()
        {
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var user = new User { Username = "friend1", Email = "friend1@example.com" };
            var friend = new User { Username = "friend2", Email = "friend2@example.com" };
            context.Users.AddRange(user, friend);
            context.Friendships.Add(new Friend { UserId = user.Id, FriendId = friend.Id });
            await context.SaveChangesAsync();

            var token = TokenManager.GenerateJWT(user);
            _client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var removeRequest = new { Id = user.Id, FriendUsername = "friend2" };
            var content = new StringContent(JsonConvert.SerializeObject(removeRequest), Encoding.UTF8, "application/json");
            
            var response = await _client.PostAsync("/api/Profile/RemoveFriend", content);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
            
            Assert.NotNull(result);
            Assert.True((bool)result.success, $"API returned failure: {responseBody}");
            Assert.Equal("Amizade removida com sucesso", (string)result.message);

            await GenericFunctions.ResetDatabase();
        }
    }
}
