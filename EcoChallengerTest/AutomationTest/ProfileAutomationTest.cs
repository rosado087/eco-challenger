using NUnit.Framework;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EcoChallenger.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Microsoft.Extensions.Configuration;
using EcoChallengerTest.Utils;

namespace EcoChallengerTest.AutomationTest
{
    public class ProfileAutomationTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private Mock<IConfiguration> _mockConfig;
        private AppDbContext _dbContext;

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
            string password = "12345678";

            TypeWithDelay(emailInput, email, 100);
            Thread.Sleep(500);

            TypeWithDelay(passwordInput, password, 100);
            Thread.Sleep(500);

            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();
            Thread.Sleep(2000);
            
            // Navigate to the profile page
            driver.Navigate().GoToUrl("http://localhost:4200/user-profile/1");

            Thread.Sleep(500);
        }

        [Test]
        public void Add_Friend_Success()
        {

            //Open Add Friend Card
            var addButton = wait.Until(d => d.FindElement(By.Id("add-friend-modal")));
            addButton.Click();

            var addPopup = wait.Until(d => d.FindElement(By.Id("add-div")));


            //Input part of username "Tes"
            var inputAdd = wait.Until(d => addPopup.FindElement(By.Id("input-add")));
            TypeWithDelay(inputAdd, "Tes", 100);

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
            var editButton = wait.Until(d => d.FindElement(By.Id("edit-info")));
            editButton.Click();

            var inputName = wait.Until(d => d.FindElement(By.Id("edit-username")));
            inputName.Clear();
            TypeWithDelay(inputName, "TestName", 100);
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
            var editButton = wait.Until(d => d.FindElement(By.Id("edit-info")));
            editButton.Click();

            var inputName = wait.Until(d => d.FindElement(By.Id("edit-username")));
            inputName.Clear();
            TypeWithDelay(inputName, "Tester2", 100);
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
            var editButton = wait.Until(d => d.FindElement(By.Id("edit-info")));
            editButton.Click();

            var selectTag = wait.Until(d => d.FindElement(By.Id("edit-tag")));
            selectTag.Click();
            
            var optionTag = wait.Until(d => d.FindElement(By.Id("option-Green Guru")));
            optionTag.Click();
            

            var saveButton = wait.Until(d => d.FindElement(By.Id("edit-save")));
            saveButton.Click();

            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();
        }

        [Test]
        public void Edit_Nothing_Profile_Success()
        {
            var editButton = wait.Until(d => d.FindElement(By.Id("edit-info")));
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
            var removeButton = wait.Until(d => d.FindElement(By.Id("view-Tester2")));
            removeButton.Click();

            wait.Until(d => d.Url.Contains("http://localhost:4200/user-profile"));
        }

        [Test]
        public void Remove_Friend_Success() {

            //Remove Friend
            var removeButton = wait.Until(d => d.FindElement(By.Id("rem-Tester2")));
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

        /*[TearDown]
        public void Teardown()
        {
            driver.Quit();
        }*/

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
