using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace EcoChallengerTest.AutomationTest
{
    public class StatisticsAutomationTest
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

            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            driver.Navigate().GoToUrl(gf.BuildUrl("/login"));

            var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));

            string email = "tester1@gmail.com";
            string password = "Password123!";

            gf.TypeWithDelay(emailInput, email);
            Thread.Sleep(500);

            gf.TypeWithDelay(passwordInput, password);
            Thread.Sleep(500);

            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();
            Thread.Sleep(2000);
        }

        [Test]
        public void View_Admin_Statistics()
        {
            var admin = wait!.Until(a => a.FindElement(By.Id("navbar-admin-dropdown-button")));
            admin.Click();
            Thread.Sleep(500);
            
            var stats = wait!.Until(a => a.FindElement(By.Id("admin-stats-navbar-button")));
            stats.Click();
            Thread.Sleep(500);
        }

        [Test]
        public void View_Profile_Statistics(){
            gf.NavigateToProfile(wait);
        }

        [TearDown]
        public void Teardown()
        {
            driver?.Quit();
        }
    }
}