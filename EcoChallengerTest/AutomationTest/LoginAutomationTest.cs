using NUnit.Framework;
using OpenQA.Selenium;

using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;

namespace EcoChallengerTest.AutomationTest
{
    public class LoginAutomationTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = GenericFunctions.SetupSeleniumInstance();

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

            string email = "testuser@example.com";
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

            var logout = wait.Until(d => d.FindElement(By.Id("user-option")));
            logout.Click();

            var nextOption = wait.Until(d => d.FindElement(By.XPath("/html/body/app-root/div[2]/app-header/div/div[2]/div/div[2]/ul/li[2]/button")));
            nextOption.Click();

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