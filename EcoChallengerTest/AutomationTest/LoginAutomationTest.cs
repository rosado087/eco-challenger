using NUnit.Framework;
using OpenQA.Selenium;

using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;
using Microsoft.AspNetCore.Mvc.Testing;

namespace EcoChallengerTest.AutomationTest
{
    [TestFixture]
    public class LoginAutomationTest
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
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            driver.Navigate().GoToUrl("http://localhost:4200/login");
        }

        [Test]
        public void Login_Success()
        {
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

            wait.Until(d => d.Url.Contains("http://localhost:4200"));
        }

        [Test]
        public void Logout_Success()
        {
            var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));

            string email = "testuser@example.com";
            string password = "Password123!";

            TypeWithDelay(emailInput, email, 100);
            Thread.Sleep(500);

            TypeWithDelay(passwordInput, password, 100);
            Thread.Sleep(500);

            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();

            wait.Until(d => d.Url.Contains("http://localhost:4200"));

            var navbarUserIcon = wait.Until(d => d.FindElement(By.Id("navbar-user-icon")));
            navbarUserIcon.Click();

            var logout = wait.Until(d => d.FindElement(By.Id("logout")));
            logout.Click();

            var logoutPopup = wait.Until(d => d.FindElement(By.XPath("/html/body/app-root/div[1]/app-popup/dialog/div")));
            
            var yesLogoutPopup = wait.Until(d => d.FindElement(By.XPath("/html/body/app-root/div[1]/app-popup/dialog/div/div/form/app-button[1]")));
            yesLogoutPopup.Click();

            wait.Until(d => d.Url.Contains("http://localhost:4200/login"));
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