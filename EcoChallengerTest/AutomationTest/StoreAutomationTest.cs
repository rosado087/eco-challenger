using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using EcoChallengerTest.Utils;

namespace EcoChallengerTest.AutomationTest
{
    public class StoreAutomationTest
    {
        private IWebDriver driver;
        private WebDriverWait wait;

        [SetUp]
        public void Setup()
        {
            driver = GenericFunctions.SetupSeleniumInstance();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
            driver.Navigate().GoToUrl("http://localhost:4200/login");
        }

        [Test]
        public void Store_TagPurchaseFlow()
        {
            // LOGIN
            var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
            var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));

            TypeWithDelay(emailInput, "tester1@gmail.com");
            TypeWithDelay(passwordInput, "12345678");

            var loginButton = wait.Until(d => d.FindElement(By.CssSelector("button[type='submit']")));
            loginButton.Click();

            // IR PARA LOJA
            var lojaLink = wait.Until(d => d.FindElement(By.LinkText("Loja")));
            lojaLink.Click();

            // TENTAR COMPRAR TAG (FALHA)
            /*var comprarTag = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(),'Comprar')]")));
            comprarTag.Click();
            Thread.Sleep(500);
            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-action']")));
            Thread.Sleep(500);
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();*/
            // Esperar notificação ou mensagem de erro (ajustar se houver alerta/modal)
            Thread.Sleep(1000);
            /*
            // IR PARA DESAFIOS
            var desafiosLink = wait.Until(d => d.FindElement(By.LinkText("Desafios")));
            desafiosLink.Click();
            /*
            // CONCLUIR TODOS DESAFIOS DIÁRIOS
            var botoesConcluir = wait.Until(d => d.FindElements(By.XPath("//h2[contains(text(),'Desafios Diários')]/following::button[contains(text(),'Concluir')]")));
            foreach (var botao in botoesConcluir)
            {
                botao.Click();
                Thread.Sleep(1000);
                popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-box']")));
                Thread.Sleep(500);
                okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

                okayButton.Click(); 
                Thread.Sleep(1000);
            }*//*

            // VOLTAR PARA LOJA
            lojaLink = wait.Until(d => d.FindElement(By.LinkText("Loja")));
            lojaLink.Click();
            */
            // COMPRAR TAG NOVAMENTE
            var comprarTag = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(),'Comprar')]")));
            comprarTag.Click();
            Thread.Sleep(1000);
            var popup = wait.Until(d => d.FindElement(By.CssSelector("div[class*='modal-box']")));
            Thread.Sleep(500);
            var okayButton = wait.Until(d => popup.FindElement(By.CssSelector("button.btn.btn-primary")));

            okayButton.Click();
            // IR PARA PERFIL
            var avatar = wait.Until(d => d.FindElement(By.CssSelector("img, .user-icon, .avatar, .profile-icon")));
            avatar.Click();
            var profileOption = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(),'Profile')]")));
            profileOption.Click();

            // SELECIONAR TAG
            var tagEcoWarrior = wait.Until(d => d.FindElement(By.XPath("//*[contains(text(),'Eco-Warrior')]")));
            tagEcoWarrior.Click();

            // BOTÃO "USAR" (AJUSTAR caso o botão tenha outro nome)
            var usarBotao = wait.Until(d => d.FindElement(By.XPath("//button[contains(text(),'Usar')]")));
            usarBotao.Click();

            // Esperar confirmação visual (pode-se usar uma asserção no futuro)
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