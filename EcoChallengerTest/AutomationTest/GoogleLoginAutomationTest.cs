using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace EcoChallengerTest.AutomationTest
{
    /*
     * This is the google login automation test but there's a problem
     * It can't access any account because it's an automation test
     

    public class GoogleLoginAutomationTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {


            // Initialize the Firefox driver
            driver = new FirefoxDriver();

            // Set up explicit wait (up to 100 seconds)
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

            driver.Navigate().GoToUrl("http://localhost:4200/login");
        }

        [Test]
        public void GoogleLogin_EndToEnd_Success()
        {
            // Step 1: Wait for the Google Login button and click it
            var googleButton = wait.Until(d => d.FindElement(By.Id("google-btn")));
            googleButton.Click();

            // Step 2: Switch to the Google login popup window
            string mainWindowHandle = driver.CurrentWindowHandle; // Save the main window handle
            foreach (string handle in driver.WindowHandles)
            {
                if (handle != mainWindowHandle)
                {
                    driver.SwitchTo().Window(handle); // Switch to the Google login popup
                    break;
                }
            }

            // Step 3: Wait for the email input field in the Google login popup
            var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[type='email']")));
            TypeWithDelay(emailInput, "testuser.automation@gmail.com", 100); // Enter test email

            // Step 4: Click the "Next" button
            var nextButton = wait.Until(d => d.FindElement(By.CssSelector("#identifierNext")));
            nextButton.Click();

            // Step 5: Wait for the password input field
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[type='password']")));
            TypeWithDelay(passwordInput, "test_autoMation2019!", 100); // Enter test password

            // Step 6: Click the "Sign in" button
            var signInButton = wait.Until(d => d.FindElement(By.CssSelector("#passwordNext")));
            signInButton.Click();

            // Step 7: Switch back to the main window
            driver.SwitchTo().Window(mainWindowHandle);

            // Step 8: Wait for login completion (e.g., redirect to dashboard)
            wait.Until(d => d.Url.Contains("http://localhost:4200"));
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
        }

        // Helper method to type text with a delay
        public void TypeWithDelay(IWebElement element, string text, int delayMilliseconds = 100)
        {
            foreach (var character in text)
            {
                element.SendKeys(character.ToString());
                Thread.Sleep(delayMilliseconds);
            }
        }
    }*/
}
    