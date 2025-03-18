using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace EcoChallengerTest.Utils
{
    public static class GenericFunctions {
        
        /// <summary>
        /// Setups a selenium instance to use for the tests
        /// </summary>
        /// <returns>Returns the browser driver</returns>
        public static WebDriver SetupSeleniumInstance() {
            // This runs the tests in headless mode
            // in other words without opening the browser
            // To debug the process, comment these two lines
            var options = new FirefoxOptions();
            options.AddArgument("--headless");

            // Initialize the Firefox driver
            return new FirefoxDriver(options);
        }
    }
}