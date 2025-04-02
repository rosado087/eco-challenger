using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Castle.Core.Configuration;
using Moq;

namespace EcoChallengerTest.AutomationTest
{
    public class AdminAutomationTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private Mock<IConfiguration> _mockConfig;
        private AppDbContext _dbContext;
        private WebApplicationFactory<Program> factory;

        [OneTimeSetUp]
        public async Task GlobalSetup()
        {
            factory = new WebApplicationFactory<Program>();
            var client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("http://localhost:5294")
            });

            GenericFunctions.Initialize(client);
            await GenericFunctions.SeedTestUsers();
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
            
            // Navigate to the profile page
            driver.Navigate().GoToUrl("http://localhost:4200/admin/tags");

            Thread.Sleep(500);
        }

        [Test]
        public void Add_Tag_Success()
        {

            var complete = wait.Until(d => d.FindElement(By.Id("addTag")));
            complete.Click();

            Thread.Sleep(1000);

            string name = "tag test";
            string price = "60";

            var nameInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='name']")));
            var priceInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='price']")));
            nameInput.Clear();
            priceInput.Clear();

            TypeWithDelay(nameInput, name, 100);
            Thread.Sleep(500);

            TypeWithDelay(priceInput, price, 100);
            Thread.Sleep(500);

            var button = wait.Until(d => d.FindElement(By.Id("submit")));
            button.Click();
        }
        [Test]
        public void Edit_Tag_Success()
        {

            var edit = wait.Until(d => d.FindElement(By.Id("editTag1")));
            edit.Click();

            Thread.Sleep(1000);

            string name = "tag test";
            string price = "60";

            var nameInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='name']")));
            var priceInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='price']")));
            nameInput.Clear();
            priceInput.Clear();

            TypeWithDelay(nameInput, name, 100);
            Thread.Sleep(500);

            TypeWithDelay(priceInput, price, 100);
            Thread.Sleep(500);

            var button = wait.Until(d => d.FindElement(By.Id("submit")));
            button.Click();
        }
        [Test]
        public void Remove_Tag_Success()
        {

            var complete = wait.Until(d => d.FindElement(By.Id("removeTag1")));
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