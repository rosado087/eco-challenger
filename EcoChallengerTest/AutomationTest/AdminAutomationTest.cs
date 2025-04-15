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
        public void Add_Tag()
        {
            var admin = wait!.Until(a => a.FindElement(By.Id("navbar-admin-dropdown-button")));
            admin.Click();
            Thread.Sleep(500);

            var tagsButton = wait!.Until(a => a.FindElement(By.Id("admin-tags-navbar-button")));
            tagsButton.Click();
            Thread.Sleep(500);

            var complete = wait!.Until(d => d.FindElement(By.Id("addTag")));
            complete.Click();

            Thread.Sleep(1000);

            string name = "tag test";
            string price = "60";

            var nameInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='name']")));
            var priceInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='price']")));
            nameInput.Clear();
            priceInput.Clear();

            gf.TypeWithDelay(nameInput, name);
            Thread.Sleep(500);

            gf.TypeWithDelay(priceInput, price);
            Thread.Sleep(500);

            var button = wait.Until(d => d.FindElement(By.Id("submit")));
            button.Click();
        }

        [Test]
        public void Edit_Tag()
        {
            var admin = wait!.Until(a => a.FindElement(By.Id("navbar-admin-dropdown-button")));
            admin.Click();
            Thread.Sleep(500);

            var tagsButton = wait!.Until(a => a.FindElement(By.Id("admin-tags-navbar-button")));
            tagsButton.Click();
            Thread.Sleep(500);

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

            gf.TypeWithDelay(nameInput, name);
            Thread.Sleep(500);

            gf.TypeWithDelay(priceInput, price);
            Thread.Sleep(500);

            var button = wait.Until(d => d.FindElement(By.Id("submit")));
            button.Click();
        }
        [Test]
        public void Remove_Tag()
        {
            var admin = wait!.Until(a => a.FindElement(By.Id("navbar-admin-dropdown-button")));
            admin.Click();
            Thread.Sleep(500);

            var tagsButton = wait!.Until(a => a.FindElement(By.Id("admin-tags-navbar-button")));
            tagsButton.Click();
            Thread.Sleep(500);

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

        [Test]
        public void Block_User()
        {
            var admin = wait!.Until(a => a.FindElement(By.Id("navbar-admin-dropdown-button")));
            admin.Click();
            Thread.Sleep(500);

            var tagsButton = wait!.Until(a => a.FindElement(By.Id("admin-users-navbar-button")));
            tagsButton.Click();
            Thread.Sleep(500);
            
            var blockButtons = driver!.FindElements(By.CssSelector("[data-action='block-user']"));
            if(blockButtons.Count == 0) throw new Exception("There are no tags that can be removed");

            blockButtons[1].Click();

            Thread.Sleep(1000);

            var confirmButton = wait!.Until(d => d.FindElement(By.Id("confirm-rem")));
            confirmButton.Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void Unblock_User()
        {
            var admin = wait!.Until(a => a.FindElement(By.Id("navbar-admin-dropdown-button")));
            admin.Click();
            Thread.Sleep(500);

            var tagsButton = wait!.Until(a => a.FindElement(By.Id("admin-users-navbar-button")));
            tagsButton.Click();
            Thread.Sleep(500);
            
            var blockButtons = driver!.FindElements(By.CssSelector("[data-action='block-user']"));
            if(blockButtons.Count == 0) throw new Exception("There are no tags that can be removed");

            blockButtons[1].Click();

            Thread.Sleep(1000);

            var confirmButton = wait!.Until(d => d.FindElement(By.CssSelector("[data-action= 'confirm-block']")));
            confirmButton.Click();
            Thread.Sleep(1000);


            blockButtons[1].Click();
            Thread.Sleep(1000);
            confirmButton.Click();
            Thread.Sleep(1000);
        }

        [Test]
        public void Add_Challenge()
        {
            var admin = wait!.Until(a => a.FindElement(By.Id("navbar-admin-dropdown-button")));
            admin.Click();
            Thread.Sleep(500);

            var challengesButton = wait!.Until(a => a.FindElement(By.Id("admin-challenges-navbar-button")));
            challengesButton.Click();
            Thread.Sleep(500);

            var add = wait!.Until(d => d.FindElement(By.Id("addChallenge")));
            add.Click();

            Thread.Sleep(1000);

            string title = "title test";
            string description = "description test";
            string points = "60";

            var titleInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='title']")));
            var descriptionInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='description']")));
            var pointsInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='points']")));

            titleInput.Clear();
            descriptionInput.Clear();
            pointsInput.Clear();

            gf.TypeWithDelay(titleInput, title);
            Thread.Sleep(500);

            gf.TypeWithDelay(descriptionInput, description);
            Thread.Sleep(500);

            gf.TypeWithDelay(pointsInput, points);
            Thread.Sleep(500);

            var button = wait.Until(d => d.FindElement(By.Id("submit")));
            button.Click();
        }

        [Test]
        public void Edit_Challenge()
        {
            var admin = wait!.Until(a => a.FindElement(By.Id("navbar-admin-dropdown-button")));
            admin.Click();
            Thread.Sleep(500);

            var challengesButton = wait!.Until(a => a.FindElement(By.Id("admin-challenges-navbar-button")));
            challengesButton.Click();
            Thread.Sleep(500);

            var edit = wait!.Until(d => d.FindElement(By.CssSelector("[data-action='editChallenge']")));
            edit.Click();

            Thread.Sleep(1000);

            string title = "title test";
            string description = "description test";
            string points = "60";

            var titleInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='title']")));
            var descriptionInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='description']")));
            var pointsInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='points']")));

            titleInput.Clear();
            descriptionInput.Clear();
            pointsInput.Clear();

            gf.TypeWithDelay(titleInput, title);
            Thread.Sleep(500);

            gf.TypeWithDelay(descriptionInput, description);
            Thread.Sleep(500);

            gf.TypeWithDelay(pointsInput, points);
            Thread.Sleep(500);

            var button = wait.Until(d => d.FindElement(By.Id("submit")));
            button.Click();
        }

        [Test]
        public void Remove_Challenge()
        {
            var admin = wait!.Until(a => a.FindElement(By.Id("navbar-admin-dropdown-button")));
            admin.Click();
            Thread.Sleep(500);

            var challengesButton = wait!.Until(a => a.FindElement(By.Id("admin-challenges-navbar-button")));
            challengesButton.Click();
            Thread.Sleep(500);

            var remove = wait!.Until(d => d.FindElement(By.CssSelector("[data-action='removeChallenge']")));
            remove.Click();
            Thread.Sleep(1000);
        }

        [TearDown]
        public void Teardown()
        {
            driver?.Quit();
        }
    }
}