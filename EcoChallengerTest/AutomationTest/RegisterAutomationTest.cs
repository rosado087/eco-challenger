using System;
using System.Threading;
using System.Threading.Tasks;
using EcoChallengerTest.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace EcoChallengerTest.AutomationTest 
{
    public class RegisterAutomationTest
    {
        private IWebDriver? driver;
        private WebDriverWait? wait;
        private readonly GenericFunctions gf = new GenericFunctions();

        [SetUp]
        public void Setup()
        {
            driver = gf.SetupSeleniumInstance();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            driver.Navigate().GoToUrl(gf.BuildUrl("/register"));
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown() {
            await gf.ResetDatabase();
        }

        [Test]
        public void RegisterNewUser_Success()
        {
            var usernameInput = wait!.Until(d => d.FindElement(By.CssSelector("input[formControlName='userName']")));
            var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));
            var registerButton = wait.Until(d => d.FindElement(By.XPath("//app-button[@text='Registar']")));
            
            gf.TypeWithDelay(usernameInput, "testuser"); 
            Thread.Sleep(500); 

            gf.TypeWithDelay(emailInput, "testuser@example.com");
            Thread.Sleep(500); 

            gf.TypeWithDelay(passwordInput, "Password123!");
            Thread.Sleep(500); 

            registerButton.Click();

            var popup = wait.Until(d => d.FindElement(By.CssSelector("app-popup")));
            var okayButton = driver!.FindElement(By.CssSelector(".modal-action button.btn.btn-primary"));
            
            okayButton.Click();
        }

        [TearDown]
        public void Teardown()
        {            
            driver?.Quit();
        }
    }
}