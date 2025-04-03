using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EcoChallengerTest.Utils;
using EcoChallenger.Utils;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace EcoChallengerTest.AutomationTest 
{
    [TestFixture]
    public class RecoverPasswordAutomationTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [OneTimeSetUp]
        public async Task OneTimeSetUpAsync() {
            driver = GenericFunctions.SetupSeleniumInstance();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

            GenericFunctions.Initialize("http://localhost:4200");

            await GenericFunctions.SeedTestUsers();
        }

        [SetUp]
        public void Setup()
        {
            driver.Navigate().GoToUrl("http://localhost:4200/forgot-password");
        }

        [Test]
        public async Task RecoverPassword_Success()
        {
            var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var sendButton = wait.Until(d => d.FindElement(By.XPath("//app-button[@text='Enviar']")));

            string emailAddress = "201902087@estudantes.ips.pt";
            TypeWithDelay(emailInput, emailAddress, 100);
            Thread.Sleep(500);

            sendButton.Click();

            // Wait for popup confirmation
            var popup = wait.Until(d => d.FindElement(By.CssSelector("app-popup")));
            var okayButton = wait.Until(d => d.FindElement(By.CssSelector(".modal-action button.btn.btn-primary")));

            // Fetch tokens from tests endpoint
            string token = await GetRecoveryTokenAsync(emailAddress);

            driver.Navigate().GoToUrl($"http://localhost:4200/reset-password/{token}");

            var newPasswordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='newPassword']")));
            var confirmPasswordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='confirmPassword']")));
            var submitButton = wait.Until(d => d.FindElement(By.XPath("//app-button[@text='Submeter']")));

            string newPassword = "NewPassword123!";
            TypeWithDelay(newPasswordInput, newPassword, 100);
            Thread.Sleep(500);
            TypeWithDelay(confirmPasswordInput, newPassword, 100);
            Thread.Sleep(500);

            submitButton.Click();

            // Verify success message
            var popup2 = wait.Until(d => d.FindElement(By.CssSelector("app-popup")));
            var okayButton2 = wait.Until(d => d.FindElement(By.CssSelector(".modal-action button.btn.btn-primary")));

            okayButton2.Click();
        }

        private async Task<string> GetRecoveryTokenAsync(string email) {
            HttpClient client = new HttpClient();

            var response = await client.GetAsync("http://localhost:4200/api/test/get-recovery-tokens");
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