using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using EcoChallenger.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Testing;
using EcoChallenger.Utils;

namespace EcoChallengerTest.Utils
{
    public static class GenericFunctions {
        private static WebApplicationFactory<Program> _factory;
        
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
        /// Initializes the service provider from the running application.
        /// </summary>
        public static void Initialize(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        /// <summary>
        /// Inserts a test user into the in-memory database used by the running application.
        /// </summary>
        public static void SeedTestUser()
        {
            if (_factory == null)
            {
                throw new InvalidOperationException("Factory has not been initialized. Call Initialize() first.");
            }

            using var scope = _factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Ensure database is created
            dbContext.Database.EnsureCreated();

            string rawPassword = "Password123!";
            string hashedPassword = PasswordGenerator.GeneratePasswordHash(rawPassword);

            var testUser = new User
            {
                Username = "TestUser",
                Email = "testuser@example.com",
                Password = hashedPassword, // Ensure password is hashed
                GoogleToken = null,
                Points = 0,
                IsAdmin = false
            };

            dbContext.Users.Add(testUser);
            dbContext.SaveChanges();
        }
    }
}