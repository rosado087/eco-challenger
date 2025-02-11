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
            var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));

            string email = "testuser@gmail.com";
            string password = "Password123!";

            TypeWithDelay(emailInput, email, 100); 
            Thread.Sleep(500); 

            TypeWithDelay(passwordInput, password, 100);
            Thread.Sleep(500); 

            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();

            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));

            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));
            okayButton.Click();

            wait.Until(d => d.Url.Contains("http://localhost:4200/")); 
        }

        [TearDown]
        public void Teardown()
        {
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