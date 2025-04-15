using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;


namespace EcoChallengerTest.Utils
{
    public class GenericFunctions {
        public static readonly string DEFAULT_TEST_URL = "http://localhost:4200";
        private HttpClient _client = new HttpClient();
        private string _baseUrl = "";

        /// <summary>
        /// Sets the base URL of the backend application.
        /// </summary>
        public GenericFunctions(string baseUrl)
        {
            _baseUrl = baseUrl.TrimEnd('/');
        }

        public GenericFunctions() : this(DEFAULT_TEST_URL) {}

        /// <summary>
        /// Setups a selenium instance to use for the tests
        /// </summary>
        /// <returns>Returns the browser driver</returns>
        public WebDriver SetupSeleniumInstance() {
            // This runs the tests in headless mode
            // in other words without opening the browser
            // To debug the process, comment these two lines
            var options = new FirefoxOptions();
            options.AddArgument("--window-size=1280,800");
            //options.AddArgument("--headless");

            // Initialize the Firefox driver
            return new FirefoxDriver(options);
        }

        /// <summary>
        /// Resets the database data, clears everything
        /// </summary>
        public async Task ResetDatabase() {
            string url = $"{_baseUrl}/api/test/reset-db";
            var response = await _client.PostAsync(url, null);

            if(!response.IsSuccessStatusCode) throw new Exception("Invalid status code reseting database");
        }

        /// <summary>
        /// Navigates to user profile
        /// </summary>
        public void NavigateToProfile(WebDriverWait wait) {
            var navbarUserIcon = wait.Until(d => d.FindElement(By.Id("navbar-user-icon")));
            navbarUserIcon.Click();

            var navbarProfileBtn = wait.Until(d => d.FindElement(By.Id("profile-navbar-button")));
            navbarProfileBtn.Click();
        }

        /// <summary>
        /// Navigates to user challenges
        /// </summary>
        public void NavigateToChallenges(WebDriverWait wait) {
            var navbarProfileBtn = wait.Until(d => d.FindElement(By.Id("challenges-navbar-button")));
            navbarProfileBtn.Click();
        }

        /// <summary>
        /// Sends a request to the seed-db action on the TestController which
        /// will fill the database with test data
        /// </summary>
        public async Task SeedDatabase()
        {        
            string url = $"{_baseUrl}/api/Test/seed-db";

            var response = await _client.PostAsync(url, null);
            
            if(!response.IsSuccessStatusCode) throw new Exception("An error occurred seeding data.");
        }

        /// <summary>
        /// Types text in a given HTML element with a small delay
        /// </summary>
        public void TypeWithDelay(IWebElement element, string text, int delayMilliseconds = 1)
        {
            foreach (var character in text)
            {
                element.SendKeys(character.ToString());
                Thread.Sleep(delayMilliseconds);
            }
        }

        /// <summary>
        /// Combines a given path with the base URL address to build a full web address
        /// </summary>
        public string BuildUrl(string path) {
            return string.Format("{0}/{1}", _baseUrl, path.TrimStart('/'));
        }
    }
}