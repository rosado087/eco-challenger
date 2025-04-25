using Newtonsoft.Json;
using EcoChallengerTest.Utils;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace EcoChallengerTest.IntegrationTest 
{
    public class ProfileControllerTests
    {
        private readonly GenericFunctions gf = new GenericFunctions();
        private readonly NetworkClient nc = new NetworkClient();

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            await gf.SeedDatabase();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown() {
            await gf.ResetDatabase();
        }

        [Test]
        public async Task GenerateToken_UserExists_ReturnsToken()
        {
            var response = await nc.SendGet("/api/Profile/GenerateToken");

            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsSuccessStatusCode, Is.True, $"API call failed: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(string.IsNullOrEmpty(content), Is.False, "Response body is empty");

            var result = JsonConvert.DeserializeObject<dynamic>(content);
            Assert.That(result, Is.Not.Null);

            Assert.That((bool)result!.success, Is.True, $"API returned failure: {content}");
            Assert.That(result.token, Is.Not.Null);
        }

        [Test]
        public async Task GetUserInfo_UserExists_ReturnsUserInfo()
        {
            var loginModel = await nc.Login();

            Assert.That(loginModel, Is.Not.Null);
            Assert.That(loginModel.user, Is.Not.Null);
            Assert.That(loginModel.user!.id > 0, "User ID was not assigned");
            Assert.That(loginModel.token, Is.Not.Null);

            var response = await nc.SendGet($"/api/Profile/GetUserInfo/{loginModel.user.id}", true, loginModel.token);
            
            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsSuccessStatusCode, Is.True, $"API call failed: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            Assert.That(string.IsNullOrEmpty(content), Is.False, "Response body is empty");

            var result = JsonConvert.DeserializeObject<dynamic>(content);
            Assert.That(result, Is.Not.Null);
            Assert.That((bool)result!.success, Is.True, $"API returned failure: {content}");
            Assert.That((string)result.username, Is.EqualTo("testuser"));
        }

        [Test]
        public async Task EditUserInfo_ValidRequest_UpdatesUser()
        {
            var content = new { Username = "newuser", Tag = "Eco" };
            var response = await nc.SendPost("/api/Profile/EditUserInfo", content, true, await nc.GetLoginToken("old@example.com"));
            
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
            
            Assert.That(result, Is.Not.Null);
            Assert.That((bool)result!.success, Is.True, $"API returned failure: {responseBody}");
            Assert.That((string)result.username, Is.EqualTo("newuser"));
        }

        [Test]
        public async Task EditUserInfo_From_Other_User()
        {
            var loginModel = await nc.Login("tester1@gmail.com");
            var content = new { Id = 4, Username = "newuser" };
            var response = await nc.SendPost("/api/Profile/EditUserInfo", content, true, loginModel.token);

            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseBody);

            Assert.That(result, Is.Not.Null);
            Assert.That((bool)result!.success, Is.True, $"API returned failure: {responseBody}");
            Assert.That((string)result.username, Is.EqualTo("newuser"));
            Assert.That((string)result.email, Is.EqualTo("tester1@gmail.com"));

            loginModel = await nc.Login("tester2@gmail.com");
            Assert.That(loginModel.user!.username, Is.EqualTo("Tester2"));
        }

        [Test]
        public async Task AddFriend_ValidRequest_AddsFriend()
        {
            var loginModel = await nc.Login("tester1@gmail.com");
            var content = new { Id = loginModel.user!.id, FriendUsername = "Tester2" };
            
            var response = await nc.SendPost("/api/Profile/AddFriend", content, true, loginModel.token);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
            
            Assert.That(result, Is.Not.Null);
            Assert.That((bool)result!.success, Is.True, $"API returned failure: {responseBody}");
        }

        [Test]
        public async Task RemoveFriend_ValidRequest_RemovesFriend()
        {
            var loginModel = await nc.Login("tester1@gmail.com");

            var content = new { Id = loginModel.user!.id, FriendUsername = "testuser" };
            
            var response = await nc.SendPost("/api/Profile/RemoveFriend", content, true, loginModel.token);
            var responseBody = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
            
            Assert.That(result, Is.Not.Null);
            Assert.That((bool)result!.success, Is.True, $"API returned failure: {responseBody}");
        }
    }
}
