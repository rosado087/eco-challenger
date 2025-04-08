using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;
using System.Threading.Tasks;
using System;
using System.Threading;

namespace EcoChallengerTest.AutomationTest
{
    public class AdminAutomationTest
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
            
            // Navigate to the profile page
            driver.Navigate().GoToUrl(gf.BuildUrl("/admin/tags"));

            Thread.Sleep(500);
        }

        [Test]
        public void Add_Tag_Success()
        {
            var complete = wait!.Until(d => d.FindElement(By.Id("addTag")));
            complete.Click();

            Thread.Sleep(1000);

            string name = "tag test";
            string price = "60";

            var nameInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='name']")));
            var priceInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='price']")));
            nameInput.Clear();
            priceInput.Clear();

            gf.TypeWithDelay(nameInput, name, 100);
            Thread.Sleep(500);

            gf.TypeWithDelay(priceInput, price, 100);
            Thread.Sleep(500);

            var button = wait.Until(d => d.FindElement(By.Id("submit")));
            button.Click();
        }
        [Test]
        public void Edit_Tag_Success()
        {
            var editButtons = driver!.FindElements(By.CssSelector("[data-action='editTag']"));
            if(editButtons.Count == 0) throw new Exception("There are no tags that can be edited");

            editButtons[0].Click();
            Thread.Sleep(1000);

            string name = "tag test";
            string price = "60";

            var nameInput = wait!.Until(d => d.FindElement(By.CssSelector("input[formControlName='name']")));
            var priceInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='price']")));
            nameInput.Clear();
            priceInput.Clear();

            gf.TypeWithDelay(nameInput, name, 100);
            Thread.Sleep(500);

            gf.TypeWithDelay(priceInput, price, 100);
            Thread.Sleep(500);

            var button = wait.Until(d => d.FindElement(By.Id("submit")));
            button.Click();
        }
        [Test]
        public void Remove_Tag_Success()
        {
            var removeButtons = driver!.FindElements(By.CssSelector("[data-action='removeTag']"));
            if(removeButtons.Count == 0) throw new Exception("There are no tags that can be removed");

            removeButtons[0].Click();

            Thread.Sleep(1000);

            var popup = wait!.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
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