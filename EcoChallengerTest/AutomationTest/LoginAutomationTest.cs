using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace EcoChallengerTest.AutomationTest
{
    public class LoginAutomationTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            // Initialize the Firefox driver
            driver = new FirefoxDriver();

            // Set up explicit wait (up to 100 seconds)
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            // Navigate to the login page
            driver.Navigate().GoToUrl("http://localhost:4200/login");
        }

        [Test]
        public void Login_Success()
        {
            // Step 1: Find the email and password input fields
            var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));

            // Step 2: Enter email and password with a delay
            string email = "testuser@gmail.com";
            string password = "Password123!";

            TypeWithDelay(emailInput, email, 100); // 100ms delay between keystrokes
            Thread.Sleep(500); // Wait before typing the next field

            TypeWithDelay(passwordInput, password, 100);
            Thread.Sleep(500); // Wait before clicking the button

            // Step 3: Find and click the "Login" button
            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();

            // Step 4: Wait for the popup to appear
            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));

            // Step 5: Find and click the "Okay" button inside the popup
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));
            okayButton.Click();

            // Step 6: Wait for the login to complete and verify success
            // Assuming the login redirects to a dashboard or home page
            wait.Until(d => d.Url.Contains("http://localhost:4200/")); // Adjust the URL as needed
        }

        [TearDown]
        public void Teardown()
        {
            // Close the browser
            driver.Quit();
        }

        // Helper method to simulate typing with a delay
        public void TypeWithDelay(IWebElement element, string text, int delayMilliseconds = 100)
        {
            foreach (var character in text)
            {
                element.SendKeys(character.ToString());
                Thread.Sleep(delayMilliseconds); // Delay between each character
            }
        }
    }
}