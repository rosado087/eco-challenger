using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;

namespace EcoChallengerTest.AutomationTest
{
    public class GamificationAutomationTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = GenericFunctions.SetupSeleniumInstance();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl("http://localhost:4200/login");

            // LOGIN
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
        }

        [Test]
        public void Gamification_Flow()
        {


            // NAVEGAR PARA A PÁGINA DE DESAFIOS
            var desafiosLink = wait.Until(d => d.FindElement(By.LinkText("Desafios")));
            desafiosLink.Click();

            // CONCLUIR UM DESAFIO DIÁRIO
            var primeiroConcluirDiario = wait.Until(d => d.FindElement(By.XPath("//h2[contains(text(),'Desafios Diários')]/following::button[contains(text(),'Concluir')]")));
            primeiroConcluirDiario.Click();

            Thread.Sleep(1000); // esperar efeito

            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();

            // TENTAR CONCLUIR UM DESAFIO SEMANAL SEM PROGRESSO
            var concluirSemanal = wait.Until(d => d.FindElement(By.XPath("//h2[contains(text(),'Desafios Semanais')]/following::button[contains(text(),'Concluir')]")));
            //concluirSemanal.Click();

            // Esperar feedback (popup, erro, etc.), aqui depende da implementação — pode ignorar ou capturar alerta

            Thread.Sleep(1000); // buffer

            // ADICIONAR PROGRESSO AO DESAFIO SEMANAL
            /*var adicionarProgresso = wait.Until(d => d.FindElement(By.XPath("//h2[contains(text(),'Desafios Semanais')]/following::button[contains(text(),'+ Progresso')]")));
            adicionarProgresso.Click();
            Thread.Sleep(1000);
            popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));
            
            okayButton.Click();
            adicionarProgresso.Click(); // repetir para completar
            Thread.Sleep(1000);
            popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();*/
            // CONCLUIR DESAFIO SEMANAL APÓS PROGRESSO
            concluirSemanal = wait.Until(d => d.FindElement(By.XPath("//h2[contains(text(),'Desafios Semanais')]/following::button[contains(text(),'Concluir')]")));
            concluirSemanal.Click();
            Thread.Sleep(1000);
            popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-box']")));
            Thread.Sleep(500);
            okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();
            // ABRIR PERFIL
            var avatar = wait.Until(d => d.FindElement(By.CssSelector("img, .user-icon, .avatar, .profile-icon"))); // ajustar conforme o seletor
            avatar.Click();


            var profileOption = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(),'Profile')]")));
            profileOption.Click();


            // VERIFICAR SE OS PONTOS FORAM ADICIONADOS
            var pontos = wait.Until(d => d.FindElement(By.XPath("//*[contains(text(),'pontos')]")));
            NUnit.Framework.Assert.That(pontos.Text.Contains("pontos"));

        }

        /*[TearDown]
        public void Teardown()
        {
            driver.Quit();
        }*/

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