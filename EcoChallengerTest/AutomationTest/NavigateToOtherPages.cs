using System;
using System.Threading.Tasks;
using EcoChallengerTest.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EcoChallengerTest.AutomationTest
{
    public class NavigateToOtherPages
    {
        private IWebDriver? driver;
        private WebDriverWait? wait;
        private readonly GenericFunctions gf = new GenericFunctions();

        [SetUp]
        public void Setup()
        {
            driver = gf.SetupSeleniumInstance();

            // Set up explicit wait (up to 100 seconds)
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            // Navigate to the home page
            driver.Navigate().GoToUrl("http://localhost:4200/");
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown() {
            await gf.ResetDatabase();
        }

        [Test]
        public void Login_To_Register()
        {
            var registerText = wait!.Until(d => d.FindElement(By.XPath("/html/body/app-root/div[2]/app-login/div/div[3]/a")));
            registerText.Click();

            wait.Until(d => d.Url.Contains("http://localhost:4200/register"));
        }

        [Test]
        public void Login_To_ForgotPassword()
        {
            var forgotText = wait!.Until(d => d.FindElement(By.XPath("/html/body/app-root/div[2]/app-login/div/div[2]/div[1]/a")));
            forgotText.Click();

            wait.Until(d => d.Url.Contains("http://localhost:4200/forgot-password"));
        }

        [Test]
        public void Register_To_Login()
        {
            var registerButton = wait!.Until(d => d.FindElement(By.XPath("/html/body/app-root/div[2]/app-header/div/div[2]/ul/li[2]/a")));
            registerButton.Click();

            wait.Until(d => d.Url.Contains("http://localhost:4200/register"));

            var loginButton = wait.Until(d => d.FindElement(By.XPath("/html/body/app-root/div[2]/app-header/div/div[2]/ul/li[1]/a")));
            loginButton.Click();
            
            wait.Until(d => d.Url.Contains("http://localhost:4200/login"));
        }

        [TearDown]
        public void Teardown()
        {
            // Close the browser
            driver?.Quit();
        }
    }
}
