using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace EcoChallengerTest.AutomationTest
{
    [TestFixture]
    public class LoginAutomationTest
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
        }

        [Test]
        public void Login_Success()
        {
            var emailInput = wait!.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));

            string email = "tester1@gmail.com";
            string password = "Password123!";

            gf.TypeWithDelay(emailInput, email);
            Thread.Sleep(500);

            gf.TypeWithDelay(passwordInput, password);
            Thread.Sleep(500);

            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();

            wait.Until(d => d.Url.Contains(GenericFunctions.DEFAULT_TEST_URL));
        }

        [Test]
        public void Logout_Success()
        {
            var emailInput = wait!.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));

            string email = "testuser@example.com";
            string password = "Password123!";

            gf.TypeWithDelay(emailInput, email);
            Thread.Sleep(500);

            gf.TypeWithDelay(passwordInput, password);
            Thread.Sleep(500);

            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();

            wait.Until(d => d.Url.Contains(GenericFunctions.DEFAULT_TEST_URL));

            var navbarUserIcon = wait.Until(d => d.FindElement(By.Id("navbar-user-icon")));
            navbarUserIcon.Click();

            var logout = wait.Until(d => d.FindElement(By.Id("logout")));
            logout.Click();

            var logoutPopup = wait.Until(d => d.FindElement(By.XPath("/html/body/app-root/div[1]/app-popup/dialog/div")));
            
            var yesLogoutPopup = wait.Until(d => d.FindElement(By.XPath("/html/body/app-root/div[1]/app-popup/dialog/div/div/form/app-button[1]")));
            yesLogoutPopup.Click();

            wait.Until(d => d.Url.Contains(gf.BuildUrl("/login")));
        }

        [TearDown]
        public void Teardown()
        {
            driver?.Quit();
        }
    }
}