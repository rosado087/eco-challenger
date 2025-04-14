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
    public class ChallengeControllerTest
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
        public async Task CreateChallenge_Valid_ShouldSucceed()
        {
            var login = await nc.Login("tester1@gmail.com");

            var form = new MultipartFormDataContent
            {
                { new StringContent("testChallenge6"), "Title" },
                { new StringContent("descricao6"), "Description" },
                { new StringContent("50"), "Points" },
                { new StringContent("Daily"), "Type" },
                { new StringContent("3"), "MaxProgress" }
            };

            var response = await nc.SendFormPost("/api/Challenge/CreateChallenge", form, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.True);
            Assert.That((string)result.message, Does.Contain("Desafio criado"));
        }

        [Test]
        public async Task CreateChallenge_Duplicate()
        {
            var login = await nc.Login("tester1@gmail.com");

            var form = new MultipartFormDataContent
            {
                { new StringContent("testChallenge1"), "Title" },
                { new StringContent("descricao1"), "Description" },
                { new StringContent("20"), "Points" },
                { new StringContent("Daily"), "Type" },
                { new StringContent("3"), "MaxProgress" }
            };

            var response = await nc.SendFormPost("/api/Challenge/CreateChallenge", form, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.False);
            Assert.That((string)result.message, Does.Contain("Já existe"));
        }

        [Test]
        public async Task CreateChallenge_AsNormalUser_ShouldFailWith403()
        {
            var login = await nc.Login();

            var form = new MultipartFormDataContent
            {
                { new StringContent("testChallenge6"), "Title" },
                { new StringContent("descricao6"), "Description" },
                { new StringContent("50"), "Points" },
                { new StringContent("Daily"), "Type" },
                { new StringContent("3"), "MaxProgress" }
            };

            var response = await nc.SendFormPost("/api/Challenge/CreateChallenge", form, true, login.token);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }


        [Test]
        public async Task GetAllChallenges_AsAdmin()
        {
            var login = await nc.Login("tester1@gmail.com");

            var response = await nc.SendGet("/api/Challenge/GetAllChallenges", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var challenges = JsonConvert.DeserializeObject<List<Challenge>>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(challenges, Is.Not.Null);
            Assert.That(challenges!.Count, Is.GreaterThanOrEqualTo(2));
            Assert.That(challenges.Any(c => c.Title == "testChallenge1"));
            Assert.That(challenges.Any(c => c.Title == "testChallenge2"));
        }

        [Test]
        public async Task GetAllChallenges_AsAdmin_Filtered()
        {
            var login = await nc.Login("tester1@gmail.com");

            var response = await nc.SendGet("/api/Challenge/GetAllChallenges?q=testChallenge1", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var challenges = JsonConvert.DeserializeObject<List<Challenge>>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(challenges, Is.Not.Null);
            Assert.That(challenges!.Count, Is.EqualTo(1));
            Assert.That(challenges.Any(c => c.Title == "testChallenge1"));
        }

        [Test]
        public async Task GetAllChallenges_AsAdmin_Filtered_No_Match()
        {
            var login = await nc.Login("tester1@gmail.com");

            var response = await nc.SendGet("/api/Challenge/GetAllChallenges?q=ola", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var challenges = JsonConvert.DeserializeObject<List<Challenge>>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(challenges, Is.Empty);
        }

        [Test]
        public async Task GetAllChallenges_AsNormalUser_ShouldFailWith403()
        {
            var login = await nc.Login();

            var response = await nc.SendGet("/api/Challenge/GetAllChallenges", true, login.token);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task EditChallenge_Valid_ShouldUpdateSuccessfully()
        {
            var login = await nc.Login("tester1@gmail.com");

            var form = new MultipartFormDataContent
            {
                { new StringContent("Updated Title"), "Title" },
                { new StringContent("Updated Description"), "Description" },
                { new StringContent("50"), "Points" },
                { new StringContent("Weekly"), "Type" },
                { new StringContent("5"), "MaxProgress" }
            };

            var responseChallenges = await nc.SendGet("/api/Challenge/GetAllChallenges", true, login.token);
            var jsonChallenges = await responseChallenges.Content.ReadAsStringAsync();
            var challenges = JsonConvert.DeserializeObject<List<Challenge>>(jsonChallenges);

            var response = await nc.SendFormPost($"/api/Challenge/EditChallenge/{challenges![0].Id}", form, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.True);
            Assert.That((string)result.message, Does.Contain("editado"));
        }

        [Test]
        public async Task EditChallenge_Same_Title()
        {
            var login = await nc.Login("tester1@gmail.com");

            var form = new MultipartFormDataContent
            {
                { new StringContent("testChallenge1"), "Title" },
                { new StringContent("Updated Description"), "Description" },
                { new StringContent("50"), "Points" },
                { new StringContent("Weekly"), "Type" },
                { new StringContent("5"), "MaxProgress" }
            };

            var responseChallenges = await nc.SendGet("/api/Challenge/GetAllChallenges", true, login.token);
            var jsonChallenges = await responseChallenges.Content.ReadAsStringAsync();
            var challenges = JsonConvert.DeserializeObject<List<Challenge>>(jsonChallenges);

            var response = await nc.SendFormPost($"/api/Challenge/EditChallenge/{challenges![0].Id}", form, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That((bool)result.success, Is.False);
            Assert.That((string)result.message, Does.Contain("este título"));
        }

        [Test]
        public async Task EditChallenge_Same_Description()
        {
            var login = await nc.Login("tester1@gmail.com");

            var form = new MultipartFormDataContent
            {
                { new StringContent("Updated Title"), "Title" },
                { new StringContent("descricao1"), "Description" },
                { new StringContent("50"), "Points" },
                { new StringContent("Weekly"), "Type" },
                { new StringContent("5"), "MaxProgress" }
            };

            var responseChallenges = await nc.SendGet("/api/Challenge/GetAllChallenges", true, login.token);
            var jsonChallenges = await responseChallenges.Content.ReadAsStringAsync();
            var challenges = JsonConvert.DeserializeObject<List<Challenge>>(jsonChallenges);

            var response = await nc.SendFormPost($"/api/Challenge/EditChallenge/{challenges![0].Id}", form, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That((bool)result.success, Is.False);
            Assert.That((string)result.message, Does.Contain("esta descrição"));
        }

        [Test]
        public async Task EditChallenge_InvalidId()
        {
            var login = await nc.Login("tester1@gmail.com");
            var invalidId = 9999;

            var form = new MultipartFormDataContent
            {
                { new StringContent("Any Title"), "Title" },
                { new StringContent("Any Desc"), "Description" },
                { new StringContent("10"), "Points" },
                { new StringContent("Daily"), "Type" },
                { new StringContent("2"), "MaxProgress" }
            };

            var response = await nc.SendFormPost($"/api/Challenge/EditChallenge/{invalidId}", form, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That((bool)result.success, Is.False);
            Assert.That((string)result.message, Does.Contain("não existe"));
        }

        [Test]
        public async Task EditChallenge_AsNormalUser_ShouldFailWith403()
        {
            var login = await nc.Login();

            var form = new MultipartFormDataContent
            {
                { new StringContent("Novo Título"), "Title" },
                { new StringContent("Nova Descrição"), "Description" },
                { new StringContent("10"), "Points" },
                { new StringContent("Daily"), "Type" },
                { new StringContent("5"), "MaxProgress" }
            };
            var response = await nc.SendFormPost($"/api/Challenge/EditChallenge/0", form, true, login.token);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task DeleteChallenge()
        {
            var login = await nc.Login("tester1@gmail.com");

            var responseChallenges = await nc.SendGet("/api/Challenge/GetAllChallenges", true, login.token);
            var jsonChallenges = await responseChallenges.Content.ReadAsStringAsync();
            var challenges = JsonConvert.DeserializeObject<List<Challenge>>(jsonChallenges);

            var response = await nc.SendPost($"/api/Challenge/DeleteChallenge/{challenges![0].Id}", null, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.True);
            Assert.That((string)result.message, Does.Contain("removido"));
        }

        [Test]
        public async Task DeleteChallenge_InvalidId()
        {
            var login = await nc.Login("tester1@gmail.com");
            var invalidId = 9999;

            var response = await nc.SendPost($"/api/Challenge/DeleteChallenge/{invalidId}", null, true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.False);
            Assert.That((string)result.message, Does.Contain("não existe"));
        }

        [Test]
        public async Task DeleteChallenge_AsNormalUser()
        {
            var login = await nc.Login();

            var response = await nc.SendPost($"/api/Challenge/DeleteChallenge/0", null, true, login.token);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task GetChallenge_ValidId()
        {
            var login = await nc.Login("tester1@gmail.com");

            var responseChallenges = await nc.SendGet("/api/Challenge/GetAllChallenges", true, login.token);
            var jsonChallenges = await responseChallenges.Content.ReadAsStringAsync();
            var challenges = JsonConvert.DeserializeObject<List<Challenge>>(jsonChallenges);

            var response = await nc.SendGet($"/api/Challenge/GetChallenge/{challenges![0].Id}", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.True);
            Assert.That((string)result.message, Does.Contain("encontrado"));
            Assert.That((string)result.challenge.title, Is.EqualTo("testChallenge1"));
        }

        [Test]
        public async Task GetChallenge_InvalidId()
        {
            var login = await nc.Login("tester1@gmail.com");

            var invalidId = 9999;

            var response = await nc.SendGet($"/api/Challenge/GetChallenge/{invalidId}", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.False);
            Assert.That((string)result.message, Does.Contain("não existe"));
        }

        [Test]
        public async Task GetChallenge_AsNormalUser()
        {
            var login = await nc.Login();

            var response = await nc.SendGet($"/api/Challenge/GetChallenge/0", true, login.token);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }


    }
}
