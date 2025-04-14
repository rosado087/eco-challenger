using Newtonsoft.Json;
using EcoChallengerTest.Utils;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Azure;
using System.Net;
using EcoChallenger.Models;

namespace EcoChallengerTest.IntegrationTest 
{
    public class UsersControllerTest
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
        public async Task GetUsers_AsAdmin_ReturnsAllUsers()
        {
            var login = await nc.Login("tester1@gmail.com");

            var response = await nc.SendGet("/api/Users/users", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(users, Is.Not.Null);
            Assert.That(users!.Any(u => u.Username == "Tester2"), Is.True);
        }

        [Test]
        public async Task GetUsers_WithSearchName_ShouldReturnFiltered()
        {
            var login = await nc.Login("tester1@gmail.com");

            var response = await nc.SendGet("/api/Users/users?searchedName=Tester2", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(users!.Count, Is.EqualTo(1));
            Assert.That(users[0].Username, Is.EqualTo("Tester2"));
        }

        [Test]
        public async Task GetUsers_AsNormalUser_ShouldFailWith403()
        {
            var login = await nc.Login();

            var response = await nc.SendGet("/api/Users/users", true, login.token);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task ChangeBlock_UserInitiallyUnblocked()
        {
            var login = await nc.Login("tester1@gmail.com");

            var responseUsers = await nc.SendGet("/api/Users/users", true, login.token);
            var jsonUsers = await responseUsers.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(jsonUsers);

            var response = await nc.SendPost($"/api/Users/block/{users![1].Id}", null, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseModel>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(result!.Success, Is.True);
            Assert.That(result.Message, Does.Contain("Estado mudado"));
        }

        [Test]
        public async Task ChangeBlock_UserInitiallyBlocked()
        {
            var login = await nc.Login("tester1@gmail.com");

            var responseUsers = await nc.SendGet("/api/Users/users", true, login.token);
            var jsonUsers = await responseUsers.Content.ReadAsStringAsync();
            var users = JsonConvert.DeserializeObject<List<User>>(jsonUsers);

            var response = await nc.SendPost($"/api/Users/block/{users![1].Id}", null, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseModel>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(result!.Success, Is.True);
        }

        [Test]
        public async Task ChangeBlock_InvalidUserId_ShouldReturnError()
        {
            var login = await nc.Login("tester1@gmail.com");
            var invalidUserId = 99999;

            var response = await nc.SendPost($"/api/Users/block/{invalidUserId}", null, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseModel>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(result!.Success, Is.False);
            Assert.That(result.Message, Does.Contain("não existe"));
        }

        [Test]
        public async Task ChangeBlock_AsNormalUser_ShouldFail()
        {
            var login = await nc.Login();

            var responseUsers = await nc.SendGet("/api/Users/users", true, login.token);

            Assert.That(responseUsers.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }
    }
}
