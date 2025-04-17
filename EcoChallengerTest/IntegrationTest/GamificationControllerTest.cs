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
    public class GamificationControllerTest 
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
        public async Task GetChallenges_WithAssignedChallenges()
        {
            var login = await nc.Login("tester1@gmail.com");
            var response = await nc.SendGet("/api/Gamification/GetChallenges", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result!.success, Is.True);
            Assert.That(result.dailyChallenges.Count, Is.GreaterThan(0));
            Assert.That(result.weeklyChallenges.Count, Is.GreaterThan(0));
        }

        [Test]
        public async Task GetChallenges_NoChallenges()
        {
            var login = await nc.Login();
            var response = await nc.SendGet("/api/Gamification/GetChallenges", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.True);
            Assert.That(result.dailyChallenges.Count, Is.EqualTo(0));
            Assert.That(result.weeklyChallenges.Count, Is.EqualTo(0));
        }


        [Test]
        public async Task CompleteChallenge()
        {
            var login = await nc.Login("tester1@gmail.com");

            var responseChallenges = await nc.SendGet("/api/Gamification/GetChallenges", true, login.token);
            var jsonChallenges = await responseChallenges.Content.ReadAsStringAsync();
            var challengesResponse = JsonConvert.DeserializeObject<ChallengesResponse>(jsonChallenges);

            var response = await nc.SendGet($"/api/Gamification/CompleteChallenge/{challengesResponse!.DailyChallenges[0].Challenge.Id}", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result!.success, Is.True);
            Assert.That((string)result.message, Does.Contain("Desafio concluido"));
        }

        [Test]
        public async Task CompleteChallenge_InvalidId()
        {
            var login = await nc.Login();
            var invalidChallengeId = 9999;

            var response = await nc.SendGet($"/api/Gamification/CompleteChallenge/{invalidChallengeId}", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.False);
            Assert.That((string)result.message, Does.Contain("não existe"));
        }

        [Test]
        public async Task CompleteChallenge_AlreadyConcluded()
        {
            var login = await nc.Login("tester1@gmail.com");
            var responseChallenges = await nc.SendGet("/api/Gamification/GetChallenges", true, login.token);
            var jsonChallenges = await responseChallenges.Content.ReadAsStringAsync();
            var challengesResponse = JsonConvert.DeserializeObject<ChallengesResponse>(jsonChallenges);

            var responseComplete = await nc.SendGet($"/api/Gamification/CompleteChallenge/{challengesResponse!.DailyChallenges[0].Challenge.Id}", true, login.token);

            var response = await nc.SendGet($"/api/Gamification/CompleteChallenge/{challengesResponse!.DailyChallenges[0].Challenge.Id}", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.False);
            Assert.That((string)result.message, Does.Contain("já está concluído"));
        }

        [Test]
        public async Task AddProgress_Valid_ShouldIncrementProgress()
        {
            var login = await nc.Login("tester1@gmail.com");
            var responseChallenges = await nc.SendGet("/api/Gamification/GetChallenges", true, login.token);
            var jsonChallenges = await responseChallenges.Content.ReadAsStringAsync();
            var challengesResponse = JsonConvert.DeserializeObject<ChallengesResponse>(jsonChallenges);

            var response = await nc.SendGet($"/api/Gamification/AddProgress/{challengesResponse!.WeeklyChallenges[0].Challenge.Id}", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result!.success, Is.True);
            Assert.That((string)result.message, Does.Contain("Progresso adicionado"));
        }

        [Test]
        public async Task AddProgress_ReachesMax_ShouldConcludeChallenge()
        {
            var login = await nc.Login("tester1@gmail.com");
            var responseChallenges = await nc.SendGet("/api/Gamification/GetChallenges", true, login.token);
            var jsonChallenges = await responseChallenges.Content.ReadAsStringAsync();
            var challengesResponse = JsonConvert.DeserializeObject<ChallengesResponse>(jsonChallenges);

            var response = await nc.SendGet($"/api/Gamification/AddProgress/{challengesResponse!.DailyChallenges[0].Challenge.Id}", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.True);
            Assert.That((string)result.message, Does.Contain("concluido"));

        }

        [Test]
        public async Task AddProgress_AlreadyConcluded()
        {
            var login = await nc.Login("tester1@gmail.com");
            var responseChallenges = await nc.SendGet("/api/Gamification/GetChallenges", true, login.token);
            var jsonChallenges = await responseChallenges.Content.ReadAsStringAsync();
            var challengesResponse = JsonConvert.DeserializeObject<ChallengesResponse>(jsonChallenges);

            var responseComplete = await nc.SendGet($"/api/Gamification/CompleteChallenge/{challengesResponse!.DailyChallenges[0].Challenge.Id}", true, login.token);

            var response = await nc.SendGet($"/api/Gamification/AddProgress/{challengesResponse!.DailyChallenges[0].Challenge.Id}", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.False);
            Assert.That((string)result.message, Does.Contain("já está concluído"));
        }

        [Test]
        public async Task AddProgress_InvalidChallenge()
        {
            var login = await nc.Login();
            var invalidId = 9999;

            var response = await nc.SendGet($"/api/Gamification/AddProgress/{invalidId}", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            dynamic result = JsonConvert.DeserializeObject<dynamic>(json)!;

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That((bool)result.success, Is.False);
            Assert.That((string)result.message, Does.Contain("não existe"));
        }
    }
}
