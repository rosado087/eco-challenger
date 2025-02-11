using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

[TestFixture]
public class RecoverPasswordAutomationTest
{
    private IWebDriver driver;
    private WebDriverWait wait;
    private AppDbContext dbContext;
    private IConfiguration configuration;

    [SetUp]
    public void Setup()
    {
        driver = new FirefoxDriver();
        wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));

        // Load configuration from appsettings.json
        var configBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        configuration = configBuilder.Build();

        // Get connection string from configuration
        string connectionString = configuration.GetConnectionString("Default");

        // Initialize the database context
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(connectionString)
            .Options;
        dbContext = new AppDbContext(options);

        driver.Navigate().GoToUrl("http://localhost:4200/forgot-password");
    }

    [Test]
    public void RecoverPassword_Success()
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

        string token = RetrieveTokenFromDatabase(emailAddress);

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

    private string RetrieveTokenFromDatabase(string emailAddress)
    {
        string? token = null;
        int attempts = 10;
        int delay = 500;

        for (int i = 0; i < attempts; i++)
        {
            var userToken = dbContext.UserTokens
                .Include(ut => ut.User)
                .FirstOrDefault(ut => ut.User.Email == emailAddress && ut.Type == UserToken.TokenType.RECOVERY);

            if (userToken != null)
            {
                token = userToken.Token;
                break;
            }

            Thread.Sleep(delay);
        }

        if (token == null)
        {
            throw new Exception($"No recovery token found for email: {emailAddress}");
        }

        return token;
    }
}
