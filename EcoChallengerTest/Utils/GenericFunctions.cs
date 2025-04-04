using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using EcoChallenger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using EcoChallenger.Utils;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NUnit.Framework.Constraints;
using OpenQA.Selenium.Support.UI;
using System.Net.Http.Headers;


namespace EcoChallengerTest.Utils
{
    public static class GenericFunctions {
        private static HttpClient _client = new HttpClient();
        private static string _baseUrl = "";

        /// <summary>
        /// Sets the base URL of the backend application.
        /// </summary>
        public static void Initialize(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
            if (string.IsNullOrEmpty(_baseUrl))
                throw new InvalidOperationException("HttpClient BaseAddress is not set.");
        }
        
        /// <summary>
        /// Setups a selenium instance to use for the tests
        /// </summary>
        /// <returns>Returns the browser driver</returns>
        public static WebDriver SetupSeleniumInstance() {
            // This runs the tests in headless mode
            // in other words without opening the browser
            // To debug the process, comment these two lines
            var options = new FirefoxOptions();
            //options.AddArgument("--headless");

            // Initialize the Firefox driver
            return new FirefoxDriver(options);
        }

        /// <summary>
        /// Resets the database data, clears everything
        /// </summary>
        public static async Task ResetDatabase() {
            if (_client == null || string.IsNullOrEmpty(_baseUrl))
                throw new InvalidOperationException("GenericFunctions not initialized correctly.");

            string url = $"{_baseUrl}/api/test/reset-db";
            var response = await _client.PostAsync(url, null);

            if(!response.IsSuccessStatusCode) throw new Exception("Invalid status code reseting database");
        }

        /// <summary>
        /// Navigates to user profile
        /// </summary>
        public static void NavigateToProfile(WebDriverWait wait) {
            var navbarUserIcon = wait.Until(d => d.FindElement(By.Id("navbar-user-icon")));
            navbarUserIcon.Click();

            var navbarProfileBtn = wait.Until(d => d.FindElement(By.Id("profile-navbar-button")));
            navbarProfileBtn.Click();
        }

        /// <summary>
        /// Navigates to user challenges
        /// </summary>
        public static void NavigateToChallenges(WebDriverWait wait) {
            var navbarProfileBtn = wait.Until(d => d.FindElement(By.Id("challenges-navbar-button")));
            navbarProfileBtn.Click();
        }

        /// <summary>
        /// Sends an HTTP request to the backend to create a test user.
        /// </summary>
        public static async Task SeedTestUsers()
        {
            if (_client == null || string.IsNullOrEmpty(_baseUrl))
                throw new InvalidOperationException("GenericFunctions not initialized correctly.");

            Console.WriteLine($" Base URL: {_baseUrl}");
    
            string registerEndpoint = $"{_baseUrl}/api/Register/RegisterAccount";

            var testUsers = new List<User>
            {
                new User { Username = "testuser", Email = "testuser@example.com", Password = "Password123!" },
                new User { Username = "testuserpasswordrecover", Email = "testuserpasswordrecover@example.com", Password = "Password123!"  },
                new User { Username = "Tester1", Email = "tester1@gmail.com", Password = "Password123!", IsAdmin = true, Points = 100 },
                new User { Username = "Tester2", Email = "tester2@gmail.com", Password = "Password123!", Points = 100 },
                new User { Username = "Tester3", Email = "tester3@gmail.com", Password = "Password123!", Points = 100 },
                new User { Username = "tester4", Email = "201902087@estudantes.ips.pt", Password = "Password123!", IsAdmin = true}
            };

            foreach (var user in testUsers)
            {                
                var response = await _client.PostAsJsonAsync(registerEndpoint, user);
                Console.WriteLine($"Status code for {user.Email}: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to register {user.Email}: {error}");
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response for {user.Email}: {jsonResponse}");

                var result = JsonSerializer.Deserialize<ResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result == null || !result.Success) {
                    Console.WriteLine($"Register failed (deserialized result was null or unsuccessful) for {user.Email}");
                    continue;
                }

            }

            string loginEndPoint = $"{_baseUrl}/api/Login/Login";

            
            var userCredenciais = new LoginRequestModel{
                Email =  testUsers[2].Email,
                Password =  testUsers[2].Password
            };

            var res = await _client.PostAsJsonAsync(loginEndPoint, userCredenciais);
            var json = await res.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<LoginResponseModel>(json, new JsonSerializerOptions{ PropertyNameCaseInsensitive = true});
            var adminToken = token.Token;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);

            string createTagsEndpoint = $"{_baseUrl}/api/Tag/create";

            var testTags = new List<TagCRUDModel>
            {
                new TagCRUDModel { Name = "Eco-Warrior", Price = 40, BackgroundColor = "#355735", TextColor = "#FFFFFF"},
                new TagCRUDModel { Name = "NatureLover", Price = 50, BackgroundColor = "#355735", TextColor = "#FFFFFF"},
                new TagCRUDModel { Name ="Green Guru", Price = 30, BackgroundColor = "#355735", TextColor = "#FFFFFF"},
            };

            foreach (var tag in testTags)
            {

                var form = new MultipartFormDataContent
                {
                    { new StringContent(tag.Name), "Name" },
                    { new StringContent(tag.Price.ToString()), "Price" },
                    { new StringContent(tag.BackgroundColor), "BackgroundColor" },
                    { new StringContent(tag.TextColor), "TextColor" },
                    // If needed: add a fake file here too
                };
                var response = await _client.PostAsync(createTagsEndpoint, form);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to create tag - StatusCode: {response.StatusCode}");
                    Console.WriteLine($"Failed to create tag '{tag.Name}' - StatusCode: {response.StatusCode}, Error: {error}");
                    continue;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result == null || !result.Success) 
                    continue;                
            }

            string createUserTagsEndpoint = $"{_baseUrl}/api/Profile/CreateTagUser";

            var userTags = new List<object>
            {
                new  { UserId = testUsers[2].Id, TagName = testTags[1].Name, SelectedTag = true} ,
                new  { UserId = testUsers[2].Id, TagName = testTags[2].Name, SelectedTag = false },
            };


            foreach (var userTag in userTags)
            {
                
                var response = await _client.PostAsJsonAsync(createUserTagsEndpoint, userTag);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to create tag - StatusCode: {response.StatusCode}");
                    Console.WriteLine($"{error}");
                    continue;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result == null || !result.Success) 
                    continue;                
            }

            string createFriendshipsEndpoint = $"{_baseUrl}/api/Profile/AddFriend";

            var friendships = new List<ProfileFriendModel>
            {
                new ProfileFriendModel { Id = testUsers[2].Id, FriendUsername = "testuser"},
                new ProfileFriendModel { Id = testUsers[2].Id, FriendUsername = "Tester3"}
            };

            foreach (var friendship in friendships)
            {
                
                var response = await _client.PostAsJsonAsync(createFriendshipsEndpoint, friendship);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to add as a friend - StatusCode: {response.StatusCode}, Error: {error}");
                    continue;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result == null || !result.Success) 
                    continue;                
            }

             string createChallengeEndpoint = $"{_baseUrl}/api/Challenge/CreateChallenge";

            var challenges = new List<ChallengeModel>
            {
                new ChallengeModel { Title = "testChallenge1", Description = "descricao1", Points = 20, Type = "Daily", UserId = testUsers[2].Id},
                new ChallengeModel { Title = "testChallenge2", Description = "descricao2", Points = 20, Type = "Daily", UserId = testUsers[2].Id},
                new ChallengeModel { Title = "testChallenge3", Description = "descricao3", Points = 20, Type = "Daily", UserId = testUsers[2].Id},
                new ChallengeModel { Title = "testChallenge4", Description = "descricao4", Points = 50, Type = "Weekly", MaxProgress = 3, UserId = testUsers[2].Id},
                new ChallengeModel { Title = "testChallenge5", Description = "descricao5", Points = 50, Type = "Weekly", MaxProgress = 4, UserId = testUsers[2].Id},
            };

            foreach (var challenge in challenges)
            {
                
                var response = await _client.PostAsJsonAsync(createChallengeEndpoint, challenge);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to add challenge - StatusCode: {response.StatusCode}, Error: {error}");
                    continue;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result == null || !result.Success) 
                    continue;                
            }
        }
    }
}