using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Castle.Core.Configuration;
using Moq;

namespace EcoChallengerTest.AutomationTest
{
    public class GamificationAutomationTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            GenericFunctions.Initialize("http://localhost:4200");
            await GenericFunctions.SeedTestUsers();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown() {
            await GenericFunctions.ResetDatabase();
        }

        [SetUp]
        public void Setup()
        {
            driver = GenericFunctions.SetupSeleniumInstance();

            // Set up explicit wait (up to 100 seconds)
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            //Perform Login
            driver.Navigate().GoToUrl("http://localhost:4200/login");

            var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));

            string email = "tester1@gmail.com";
            string password = "Password123!";

            TypeWithDelay(emailInput, email, 100);
            Thread.Sleep(500);

            TypeWithDelay(passwordInput, password, 100);
            Thread.Sleep(500);

            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();
            Thread.Sleep(2000);
            
            GenericFunctions.NavigateToChallenges(wait);

            Thread.Sleep(500);
        }

        

        [Test]
        public void Complete_Challenge_Success()
        {

            var complete = wait.Until(d => d.FindElement(By.Id("complete-testChallenge1")));
            complete.Click();

            Thread.Sleep(1000);

            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();

            GenericFunctions.NavigateToProfile(wait);
            Thread.Sleep(1000);
        }

        [Test]
        public void AddProgress_Success()
        {

            var complete = wait.Until(d => d.FindElement(By.Id("progress-testChallenge4")));
            complete.Click();

            Thread.Sleep(1000);

            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);

            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));
            okayButton.Click();

            Thread.Sleep(1000);
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }

        private void TypeWithDelay(IWebElement element, string text, int delayMilliseconds = 100)
        {
            foreach (var character in text)
            {
                element.SendKeys(character.ToString());
                Thread.Sleep(delayMilliseconds);
            }
        }
    }
}