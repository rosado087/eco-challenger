using Microsoft.AspNetCore.Routing;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
public class RegisterAutomationTest
{
    private IWebDriver driver;
    private WebDriverWait wait;

    [SetUp]
    public void Setup()
    {
        driver = new FirefoxDriver();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(100));

        driver.Navigate().GoToUrl("http://localhost:4200/register");
    }

    [Test]
    public void RegisterNewUser_Success()
    {
        
        var usernameInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='userName']")));
        var emailInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='email']")));
        var passwordInput = wait.Until(d => d.FindElement(By.CssSelector("input[formControlName='password']")));
        var registerButton = wait.Until(d => d.FindElement(By.XPath("//app-button[@text='Registar']")));
        
        TypeWithDelay(usernameInput, "testuser", 100); 
        Thread.Sleep(500); 

        TypeWithDelay(emailInput, "testuser@example.com", 100);
        Thread.Sleep(500); 

        TypeWithDelay(passwordInput, "Password123!", 100);
        Thread.Sleep(500); 

        registerButton.Click();

        var popup = wait.Until(d => d.FindElement(By.CssSelector("app-popup")));
        var okayButton = driver.FindElement(By.CssSelector(".modal-action button.btn.btn-primary"));
        
        okayButton.Click();
    }

    [TearDown]
    public void Teardown()
    {
        
        driver.Quit();
    }
    public void TypeWithDelay(IWebElement element, string text, int delayMilliseconds = 100)
    {
        foreach (var character in text)
        {
            element.SendKeys(character.ToString());
            Thread.Sleep(delayMilliseconds);
        }
    }
}