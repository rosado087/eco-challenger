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

namespace EcoChallengerTest.Utils
{
    public static class GenericFunctions {
        private static HttpClient _client = new HttpClient();
        private static string _baseUrl;

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
                Console.WriteLine($"Registering user: {user.Email}");
                
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

                Console.WriteLine($"Successfully registered: {user.Email}");
            }

            string createTagsEndpoint = $"{_baseUrl}/api/Profile/CreateTag";

            var testTags = new List<Tag>
            {
                new Tag { Name = "Eco-Warrior", BackgroundColor = "#355735", Price = 40, TextColor = "#FFFFFF"},
                new Tag { Name = "NatureLover", BackgroundColor = "#355735", Price = 50, TextColor = "#FFFFFF"},
                new Tag { Name ="Green Guru", BackgroundColor = "#355735", Price = 30, TextColor = "#FFFFFF"},
            };

            foreach (var tag in testTags)
            {
                
                var response = await _client.PostAsJsonAsync(createTagsEndpoint, tag);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
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
                new  { UserId = 3, TagId = 1, SelectedTag = true },
                new  { UserId = 3, TagId = 2, SelectedTag = false} ,
                new  { UserId = 3, TagId = 3, SelectedTag = false },
            };


            foreach (var userTag in userTags)
            {
                
                var response = await _client.PostAsJsonAsync(createUserTagsEndpoint, userTag);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to create tag - StatusCode: {response.StatusCode}, Error: {error}");
                    continue;
                }

                var jsonResponse = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<ResponseModel>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (result == null || !result.Success) 
                    continue;                
            }

            string createFriendshipsEndpoint = $"{_baseUrl}/api/Profile/AddFriend";

            var friendships = new List<object>
            {
                new { Id = 3, FriendUsername = "testuser"},
                new { Id = 3, FriendUsername = "Tester3"}
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

            var challenges = new List<object>
            {
                new  { Title = "testChallenge1", Description = "descricao1", Points = 20, Type = "Daily", UserId = 3},
                new  { Title = "testChallenge2", Description = "descricao2", Points = 20, Type = "Daily", UserId = 3},
                new  { Title = "testChallenge3", Description = "descricao3", Points = 20, Type = "Daily", UserId = 3},
                new  { Title = "testChallenge4", Description = "descricao4", Points = 50, Type = "Weekly", MaxProgress = 3, UserId = 3},
                new  { Title = "testChallenge5", Description = "descricao5", Points = 50, Type = "Weekly", MaxProgress = 4, UserId = 3},
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