using NUnit.Framework;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using EcoChallengerTest.Utils;
using System.Threading;
using System;
using System.Threading.Tasks;

namespace EcoChallengerTest.AutomationTest
{
    public class ProfileAutomationTest
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

            gf.TypeWithDelay(emailInput, email);
            Thread.Sleep(500);

            gf.TypeWithDelay(passwordInput, password);
            Thread.Sleep(500);

            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();
            Thread.Sleep(2000);
            
            gf.NavigateToProfile(wait);

            Thread.Sleep(500);
        }
        

        [Test]
        public void Add_Friend_Success()
        {

            //Open Add Friend Card
            var addButton = wait!.Until(d => d.FindElement(By.Id("add-friend-modal")));
            addButton.Click();

            var addPopup = wait.Until(d => d.FindElement(By.Id("add-div")));

            //Input part of username "Tes"
            var inputAdd = wait.Until(d => addPopup.FindElement(By.Id("input-add")));
            gf.TypeWithDelay(inputAdd, "Tes");

            //Select a user
            var selectUser = wait.Until(d => addPopup.FindElement(By.Id("pos-Tester2")));
            selectUser.Click();
            //Add Friend
            var addFriendButton = wait.Until(d => addPopup.FindElement(By.Id("add-friend-modal")));

            addFriendButton.Click();
            Thread.Sleep(500);
            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();
        }
        
        [Test]
        public void Edit_Username_Profile_Success()
        {
            var editButton = wait!.Until(d => d.FindElement(By.Id("edit-info")));
            editButton.Click();

            var inputName = wait.Until(d => d.FindElement(By.Id("edit-username")));
            inputName.Clear();
            gf.TypeWithDelay(inputName, "TestName");
            Thread.Sleep(500);

            var saveButton = wait.Until(d => d.FindElement(By.Id("edit-save")));
            saveButton.Click();
            Thread.Sleep(500);
            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();
        }

        [Test]
        public void Edit_Username_Profile_Failure()
        {
            var editButton = wait!.Until(d => d.FindElement(By.Id("edit-info")));
            editButton.Click();

            var inputName = wait.Until(d => d.FindElement(By.Id("edit-username")));
            inputName.Clear();
            gf.TypeWithDelay(inputName, "Tester2");
            Thread.Sleep(500);

            var saveButton = wait.Until(d => d.FindElement(By.Id("edit-save")));
            saveButton.Click();
            Thread.Sleep(500);
            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();
        }

        [Test]
        public void Edit_Tag_Profile_Success()
        {
            var editButton = wait!.Until(d => d.FindElement(By.Id("edit-tags")));
            editButton.Click();
            
            var tagList = wait.Until(d => d.FindElements(By.CssSelector("[data-action='tag-check-option']")));
            Assert.That(tagList.Count(), Is.GreaterThanOrEqualTo(2));
            
            tagList[1].Click();

            var submitButton = wait.Until(d => d.FindElement(By.Id("tag-select-submit")));
            submitButton.Click();

            Thread.Sleep(200);

            var profileTagListContainer = wait.Until(d => d.FindElement(By.Id("profile-tag-list")));
            Assert.That(profileTagListContainer.FindElements(
                    By.CssSelector("[role='button']")).Count(), 
                    Is.EqualTo(2));
        }

        [Test]
        public void Edit_Nothing_Profile_Success()
        {
            var editButton = wait!.Until(d => d.FindElement(By.Id("edit-info")));
            editButton.Click();

            var saveButton = wait.Until(d => d.FindElement(By.Id("edit-save")));
            saveButton.Click();


            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();
        }

        [Test]
        public void View_Friend_Profile_Success()
        {
            //View Friend Profile
            var removeButton = wait!.Until(d => d.FindElement(By.CssSelector("[data-role='view-profile-button']")));
            removeButton.Click();

            wait.Until(d => d.Url.Contains(gf.BuildUrl("/user-profile")));
        }

        [Test]
        public void Remove_Friend_Success() {

            //Remove Friend
            var removeButton = wait!.Until(d => d.FindElement(By.Id("rem-tester4")));
            removeButton.Click();
            Thread.Sleep(500);

            //Confirm Removing Friend
            var confirmButton = wait.Until(d => d.FindElement(By.Id("confirm-rem")));
            confirmButton.Click();

            Thread.Sleep(500);
            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));

            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));
            okayButton.Click();
        }

        [TearDown]
        public void Teardown()
        {
            driver?.Quit();
        }
    }
}
