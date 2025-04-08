using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace EcoChallengerTest.AutomationTest
{
    public class GamificationAutomationTest
    {
        private IWebDriver? driver;
        private WebDriverWait? wait;
        private readonly GenericFunctions gf = new GenericFunctions();

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            await gf.SeedDatabase();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown() {
            await gf.ResetDatabase();
        }

        [SetUp]
        public void Setup()
        {
            driver = gf.SetupSeleniumInstance();

            // Set up explicit wait (up to 100 seconds)
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            //Perform Login
            driver.Navigate().GoToUrl(gf.BuildUrl("/login"));

            var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));

            string email = "tester1@gmail.com";
            string password = "Password123!";

            gf.TypeWithDelay(emailInput, email, 100);
            Thread.Sleep(500);

            gf.TypeWithDelay(passwordInput, password, 100);
            Thread.Sleep(500);

            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();
            Thread.Sleep(2000);
            
            gf.NavigateToChallenges(wait);

            Thread.Sleep(500);
        }

        [Test]
        public void Complete_Challenge_Success()
        {

            var complete = wait!.Until(d => d.FindElement(By.CssSelector("[data-role='challenge-complete-button']")));
            complete.Click();

            Thread.Sleep(1000);

            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();

            gf.NavigateToProfile(wait);
            Thread.Sleep(1000);
        }

        [Test]
        public void AddProgress_Success()
        {

            var complete = wait!.Until(d => d.FindElement(By.CssSelector("[data-role='challenge-progress-button']")));
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
            driver?.Quit();
        }
    }
}