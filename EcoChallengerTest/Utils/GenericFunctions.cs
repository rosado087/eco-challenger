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

namespace EcoChallengerTest.Utils
{
    public static class GenericFunctions {
        private static HttpClient _client = new HttpClient();
        private static string _baseUrl;

        /// <summary>
        /// Sets the base URL of the backend application.
        /// </summary>
        public static void Initialize(HttpClient client)
        {
            _baseUrl = client.BaseAddress?.ToString()?.TrimEnd('/');
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
            // options.AddArgument("--headless");

            // Initialize the Firefox driver
            return new FirefoxDriver(options);
        }

        /// <summary>
        /// Sends an HTTP request to the backend to create a test user.
        /// </summary>
        public static async Task SeedTestUsers()
        {
            if (_client == null || string.IsNullOrEmpty(_baseUrl))
                throw new InvalidOperationException("GenericFunctions not initialized correctly.");

            Console.WriteLine($"🌐 Base URL: {_baseUrl}");
    
            string registerEndpoint = $"{_baseUrl}/api/Register/RegisterAccount";

            var testUsers = new List<User>
            {
                new User { Username = "testuser", Email = "testuser@example.com", Password = "Password123!" },
                new User { Username = "testuserpasswordrecover", Email = "testuserpasswordrecover@example.com", Password = "Password123!"  },
                new User { Username = "Tester1", Email = "tester1@gmail.com", Password = "Password123!" },
                new User { Username = "Tester2", Email = "tester2@gmail.com", Password = "Password123!" },
                new User { Username = "Tester3", Email = "tester3@gmail.com", Password = "Password123!" }
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

            string createTagsEndpoint = $"{_baseUrl}/api/Profile/CreateTags";

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

            string createUserTagsEndpoint = $"{_baseUrl}/api/Profile/CreateTags";

            var userTags = new List<TagUsers>
            {
                new TagUsers { User = testUsers[2], Tag = testTags[0], SelectedTag = true },
                new TagUsers { User = testUsers[2], Tag = testTags[1], SelectedTag = false} ,
                new TagUsers { User = testUsers[2], Tag = testTags[2], SelectedTag = false },
            };

            foreach (var userTag in userTags)
            {
                
                var response = await _client.PostAsJsonAsync(createUserTagsEndpoint, userTag);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Failed to create tag '{userTag.User}' - StatusCode: {response.StatusCode}, Error: {error}");
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