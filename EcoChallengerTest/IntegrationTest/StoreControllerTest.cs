using Newtonsoft.Json;
using EcoChallengerTest.Utils;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Azure;

namespace EcoChallengerTest.IntegrationTest 
{
    public class StoreControllerTest
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
        public async Task Get_Store_Tags_Success(){

            var loginModel = await nc.Login();

            Assert.That(loginModel, Is.Not.Null);
            Assert.That(loginModel.user, Is.Not.Null);
            Assert.That(loginModel.user!.id > 0, "User ID was not assigned");
            Assert.That(loginModel.token, Is.Not.Null);

            var response = await nc.SendGet("/api/Store/tags", true, loginModel.token);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.IsSuccessStatusCode, Is.True, $"API call failed: {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<UserTag>>(content);

            Assert.That(tags, Is.Not.Null);
            Assert.That(tags.Count, Is.GreaterThan(0), "No tags were returned");

            foreach (var tag in tags)
            {
                Assert.That(tag.Id, Is.GreaterThan(0));
                Assert.That(tag.Name, Is.Not.Null.And.Not.Empty);
                Assert.That(tag.IsBeingUsed, Is.TypeOf<bool>());
            }
        }

        [Test]
        public async Task Purchase_Tag_Success()
        {
            var loginModel = await nc.Login();

            Assert.That(loginModel, Is.Not.Null);
            Assert.That(loginModel.user, Is.Not.Null);
            Assert.That(loginModel.user!.id > 0, "User ID was not assigned");
            Assert.That(loginModel.token, Is.Not.Null);

            var responseTags = await nc.SendGet("/api/Store/tags", true, loginModel.token);
            var content = await responseTags.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<UserTag>>(content);

            var response = await nc.SendPost($"/api/Store/purchase/{tags![1].Id}");

            Assert.That(response.IsSuccessStatusCode, Is.True);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseModel>(json);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Success, Is.True);
        }

        [Test]
        public async Task Purchase_Tag_Already_Owned()
        {
            var loginModel = await nc.Login();

            var responseTags = await nc.SendGet("/api/Store/tags", true, loginModel.token);
            var content = await responseTags.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<Tag>>(content);
            var tag = tags[0].Id;
            var response = await nc.SendPost($"/api/Store/purchase/{tag}");
            
            Assert.That(response.IsSuccessStatusCode, Is.True);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseModel>(json);

            Assert.That(result!.Success, Is.False);
            Assert.That(result.Message, Does.Contain("já comprou esta tag"));
        }


        [Test]
        public async Task Purchase_Tag_InsufficientPoints()
        {
            var loginModel = await nc.Login("tester5@gmail.com");

            var responseTags = await nc.SendGet("/api/Store/tags", true, loginModel.token);
            var content = await responseTags.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<UserTag>>(content);

            var response = await nc.SendPost($"/api/Store/purchase/{tags[0].Id}", null, true, loginModel.token);

            Assert.That(response.IsSuccessStatusCode, Is.True);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseModel>(json);

            Assert.That(result!.Success, Is.False);
            Assert.That(result.Message, Does.Contain("não tem pontos suficientes"));
        }

    }
}
