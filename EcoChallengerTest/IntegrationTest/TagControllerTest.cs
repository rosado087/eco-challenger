using Newtonsoft.Json;
using EcoChallengerTest.Utils;
using NUnit.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Azure;
using System.Net;
using EcoChallenger.Utils;

namespace EcoChallengerTest.IntegrationTest 
{
    public class TagControllerTest
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
        public async Task Create_Tag_Success()
        {
            var loginModel = await nc.Login("tester1@gmail.com");

            var form = new MultipartFormDataContent
            {
                { new StringContent("EcoTag Test"), "Name" },
                { new StringContent("50"), "Price" },
                { new StringContent("#ffffff"), "BackgroundColor" },
                { new StringContent("#000000"), "TextColor" },
                { new StringContent("0"), "Style" }
            };

            var response = await nc.SendFormPost("/api/Tag/create", form, true, loginModel.token);

            Assert.That(response.IsSuccessStatusCode, Is.True);

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseModel>(content);

            Assert.That(result, Is.Not.Null);
            Assert.That(result!.Success, Is.True);
        }

        [Test]
        public async Task Create_Tag_DuplicateName()
        {
            var loginModel = await nc.Login("tester1@gmail.com");

            var form = new MultipartFormDataContent
            {
                { new StringContent("NatureLover"), "Name" },
                { new StringContent("50"), "Price" },
                { new StringContent("#355735"), "BackgroundColor" },
                { new StringContent("#FFFFFF"), "TextColor" },
                { new StringContent("0"), "Style" }
            };

            var response = await nc.SendFormPost("/api/Tag/create", form, true, loginModel.token);


            var content = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseModel>(content);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest));
            Assert.That(result!.Success, Is.False);
            Assert.That(result.Message, Does.Contain("já existe"));
        }


        [Test]
        public async Task Create_Tag_UnauthorizedUser()
        {
            var login = await nc.Login();

            var form = new MultipartFormDataContent
            {
                { new StringContent("EcoTag Test"), "Name" },
                { new StringContent("50"), "Price" },
                { new StringContent("#ffffff"), "BackgroundColor" },
                { new StringContent("#000000"), "TextColor" },
                { new StringContent("0"), "Style" }
            };

            var response = await nc.SendFormPost("/api/Tag/create", form, true, login.token);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }

        [Test]
        public async Task GetTags_ReturnsAllTags()
        {
            var loginModel = await nc.Login("tester1@gmail.com");
            var response = await nc.SendGet("/api/Tag/all", true, loginModel.token);
            var content = await response.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<Tag>>(content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(tags, Is.Not.Null);
            Assert.That(tags!.Count, Is.GreaterThanOrEqualTo(1));
        }

        [Test]
        public async Task GetAllTags_ReturnsFilteredTags()
        {
            var loginModel = await nc.Login("tester1@gmail.com");
            var response = await nc.SendGet("/api/Tag/all?q=NatureLover", true, loginModel.token);
            var content = await response.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<Tag>>(content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(tags, Is.Not.Null);
            Assert.That(tags!.Count, Is.EqualTo(1));
            Assert.That(tags[0].Name, Is.EqualTo("NatureLover"));
        }

        [Test]
        public async Task GetAllTags_ReturnsEmpty()
        {
            var loginModel = await nc.Login("tester1@gmail.com");
            var response = await nc.SendGet("/api/Tag/all?q=ola", true, loginModel.token);
            var content = await response.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<Tag>>(content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(tags, Is.Empty);
        }

        [Test]
        public async Task GetTag_ById_ReturnsTag()
        {
            var loginModel = await nc.Login("tester1@gmail.com");
            var responseTags = await nc.SendGet("/api/Tag/all", true, loginModel.token);
            var contentTags = await responseTags.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<Tag>>(contentTags);

            var response = await nc.SendGet($"/api/Tag/per-id/{tags![0].Id}", true, loginModel.token);
            var content = await response.Content.ReadAsStringAsync();
            var tag = JsonConvert.DeserializeObject<Tag>(content);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(tag, Is.Not.Null);
            Assert.That(tag!.Id, Is.EqualTo(tags[0].Id));
            Assert.That(tag.Name, Is.EqualTo(tags[0].Name));
        }

        [Test]
        public async Task GetTag_ByInvalidId_ShouldReturn404()
        {
            var loginModel = await nc.Login("tester1@gmail.com");
            var nonExistentId = 9999;

            var response = await nc.SendGet($"/api/Tag/per-id/{nonExistentId}", true, loginModel.token);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Does.Contain("A tag with the given ID was not found"));
        }

        [Test]
        public async Task RemoveTag_ValidId_ShouldSucceed()
        {
            var loginModel = await nc.Login("tester1@gmail.com");
            var responseTags = await nc.SendGet("/api/Tag/all", true, loginModel.token);
            var contentTags = await responseTags.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<Tag>>(contentTags);

            var response = await nc.SendPost($"/api/Tag/remove/{tags![0].Id}", null, true, loginModel.token);

            Assert.That(response.IsSuccessStatusCode, Is.True);

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ResponseModel>(json);
            Assert.That(result!.Success, Is.True);
        }

        [Test]
        public async Task RemoveTag_InvalidId_ShouldReturn404()
        {
            var loginModel = await nc.Login("tester1@gmail.com");
            var invalidId = 9999;

            var response = await nc.SendPost($"/api/Tag/remove/{invalidId}", null, true, loginModel.token);
            var content = await response.Content.ReadAsStringAsync();

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            Assert.That(content, Does.Contain("There is no tag with the given ID."));
        }

        [Test]
        public async Task RemoveTag_AsNormalUser_ShouldFailWith403()
        {
            var loginModel = await nc.Login();
            var responseTags = await nc.SendGet("/api/Tag/all", true, loginModel.token);
            var contentTags = await responseTags.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<Tag>>(contentTags);

            var response = await nc.SendPost($"/api/Tag/remove/{tags![0].Id}", null, true, loginModel.token);

            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Forbidden));
        }
    }
}
