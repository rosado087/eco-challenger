using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;
using System.Text.Json;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;

namespace EcoChallengerTest.AutomationTest 
{
    [TestFixture]
    public class RecoverPasswordAutomationTest
    {
        private IWebDriver? driver;
        private WebDriverWait? wait;
        private readonly GenericFunctions gf = new GenericFunctions();

        [OneTimeSetUp]
        public async Task OneTimeSetUpAsync() {
            driver = gf.SetupSeleniumInstance();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            await gf.SeedDatabase();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown() {
            await gf.ResetDatabase();
        }

        [SetUp]
        public void Setup()
        {
            driver!.Navigate().GoToUrl(gf.BuildUrl("/forgot-password"));
        }

        [Test]
        public async Task RecoverPassword_Success()
        {
            var emailInput = wait!.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var sendButton = wait.Until(d => d.FindElement(By.XPath("//app-button[@text='Enviar']")));

            string emailAddress = "201902087@estudantes.ips.pt";
            gf.TypeWithDelay(emailInput, emailAddress, 100);
            Thread.Sleep(500);

            sendButton.Click();

            // Wait for popup confirmation
            var popup = wait.Until(d => d.FindElement(By.CssSelector("app-popup")));
            var okayButton = wait.Until(d => d.FindElement(By.CssSelector(".modal-action button.btn.btn-primary")));

            // Fetch tokens from tests endpoint
            string token = await GetRecoveryTokenAsync(emailAddress);

            driver!.Navigate().GoToUrl(gf.BuildUrl($"/reset-password/{token}"));

            var newPasswordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='newPassword']")));
            var confirmPasswordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='confirmPassword']")));
            var submitButton = wait.Until(d => d.FindElement(By.XPath("//app-button[@text='Submeter']")));

            string newPassword = "NewPassword123!";
            gf.TypeWithDelay(newPasswordInput, newPassword, 100);
            Thread.Sleep(500);
            gf.TypeWithDelay(confirmPasswordInput, newPassword, 100);
            Thread.Sleep(500);

            submitButton.Click();

            // Verify success message
            var popup2 = wait.Until(d => d.FindElement(By.CssSelector("app-popup")));
            var okayButton2 = wait.Until(d => d.FindElement(By.CssSelector(".modal-action button.btn.btn-primary")));

            okayButton2.Click();
        }

        private async Task<string> GetRecoveryTokenAsync(string email) {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync(gf.BuildUrl("/api/test/get-recovery-tokens"));
            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonResponse, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
            if(result == null) throw new ArgumentException("Could not get recovery token");

            result.TryGetValue(email, out string? token);

            if(string.IsNullOrEmpty(token)) throw new ArgumentException("Recovery token is empty");

            return token;
        }

        [TearDown]
        public void Teardown()
        {
            driver?.Quit();
        }
    }
}