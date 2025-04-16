using EcoChallengerTest.Utils;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EcoChallengerTest.IntegrationTest
{
    public class StatisticsControllerTest
    {
        private readonly GenericFunctions gf = new GenericFunctions();
        private readonly NetworkClient nc = new NetworkClient();

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            await gf.SeedDatabase();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await gf.ResetDatabase();
        }



        [Test]
        public async Task Get_Top_Tags_Purchased_Successfully()
        {
            var login = await nc.Login("tester1@gmail.com");
            
            var response = await nc.SendGet("/api/Statistics/top-purchased-tags", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var topResponse = JsonConvert.DeserializeObject<TopInfoTagResponse>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(topResponse!.Tags.Count(), Is.LessThanOrEqualTo(10));
            Assert.That(topResponse!.Tags[0].Count >= topResponse!.Tags[1].Count, Is.True);

        }
        
        [Test]
        public async Task Get_Top_Tags_Purchased_Normal_User()
        {
            var login = await nc.Login("tester3@gmail.com");

            var response = await nc.SendGet("/api/Statistics/top-purchased-tags", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var topResponse = JsonConvert.DeserializeObject<TopInfoTagResponse>(json);

            Assert.That(response.IsSuccessStatusCode, Is.False);

        }
        
        [Test]
        public async Task Get_Top_Challenges_Completed_Successfully()
        {
            var login = await nc.Login("tester1@gmail.com");
            
            var response = await nc.SendGet("/api/Statistics/top-completed-challenges", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var topResponse = JsonConvert.DeserializeObject<TopInfoChallengeResponse>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(topResponse!.Challenges.Count(), Is.LessThanOrEqualTo(10));
            Assert.That(topResponse!.Challenges[0].Count >= topResponse!.Challenges[1].Count, Is.True);

        }

        [Test]
        public async Task Get_Top_Challenges_Completed_Normal_User()
        {
            var login = await nc.Login("tester3@gmail.com");

            var response = await nc.SendGet("/api/Statistics/top-completed-challenges", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var topResponse = JsonConvert.DeserializeObject<TopInfoChallengeResponse>(json);

            Assert.That(response.IsSuccessStatusCode, Is.False);

        }

        [Test]
        public async Task Get_Top_Most_Points_Successfully()
        {
            var login = await nc.Login("tester1@gmail.com");

            var response = await nc.SendGet("/api/Statistics/top-users-most-points", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var topResponse = JsonConvert.DeserializeObject<TopInfoUserResponse>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(topResponse!.Users.Count(), Is.LessThanOrEqualTo(5));
            Assert.That(topResponse!.Users[0].Points >= topResponse!.Users[1].Points, Is.True);

        }

        [Test]
        public async Task Get_Top_Most_Points_Normal_User()
        {
            var login = await nc.Login("tester3@gmail.com");

            var response = await nc.SendGet("/api/Statistics/top-users-most-points", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var topResponse = JsonConvert.DeserializeObject<TopInfoUserResponse>(json);

            Assert.That(response.IsSuccessStatusCode, Is.False);

        }

        [Test]
        public async Task Get_Friend_Top_Most_Points_Successfully()
        {
            var login = await nc.Login("tester1@gmail.com");

            var response = await nc.SendGet("/api/Statistics/top-friends-most-points", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var topResponse = JsonConvert.DeserializeObject<TopInfoUserResponse>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(topResponse!.Users.Count(), Is.LessThanOrEqualTo(5));
            Assert.That(topResponse!.Users[0].Points >= topResponse!.Users[1].Points, Is.True);

        }
        
        [Test]
        public async Task Get_Friend_Top_Most_Points_No_Friends()
        {
            var login = await nc.Login("tester3@gmail.com");

            var response = await nc.SendGet("/api/Statistics/top-friends-most-points", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var topResponse = JsonConvert.DeserializeObject<TopInfoUserResponse>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(topResponse!.Users.Count(), Is.EqualTo(0));
        }


        [Test]
        public async Task Get_Monthly_Users_Successfully()
        {
            var login = await nc.Login("tester1@gmail.com");

            var response = await nc.SendGet("/api/Statistics/monthly-active-users", true, login.token);
            var json = await response.Content.ReadAsStringAsync();
            var topResponse = JsonConvert.DeserializeObject<TopInfoMonthlyResponse>(json);

            Assert.That(response.IsSuccessStatusCode, Is.True);
            Assert.That(topResponse!.Logins.Count(), Is.EqualTo(1));

        }
    }
}
