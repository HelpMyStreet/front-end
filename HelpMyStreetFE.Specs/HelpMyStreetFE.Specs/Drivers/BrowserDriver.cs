using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Drivers
{
    public class BrowserDriver : IDisposable
    {
        private readonly Lazy<IWebDriver> _currentWebDriverLazy;
        private readonly ScenarioContext _scenarioContext;
        private bool _isDisposed;

        public BrowserDriver(ScenarioContext scenarioContext)
        {
            _currentWebDriverLazy = new Lazy<IWebDriver>(CreateWebDriver);
            _scenarioContext = scenarioContext;
        }

        /// <summary>
        /// The Selenium IWebDriver instance
        /// </summary>
        public IWebDriver Current => _currentWebDriverLazy.Value;

        /// <summary>
        /// Creates the Selenium web driver (opens a browser)
        /// </summary>
        /// <returns></returns>
        private IWebDriver CreateWebDriver()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            ChromeOptions chromeCapability = new ChromeOptions();
            chromeCapability.AddAdditionalCapability("os_version", "10", true);
            chromeCapability.AddAdditionalCapability("browser", "Chrome", true);
            chromeCapability.AddAdditionalCapability("browser_version", "latest", true);
            chromeCapability.AddAdditionalCapability("os", "Windows", true);
            chromeCapability.AddAdditionalCapability("name", _scenarioContext.ScenarioInfo.Title, true);
            chromeCapability.AddAdditionalCapability("build", $"Local test build {version} run {_scenarioContext["test-run-id"]}", true);

            IWebDriver driver = new RemoteWebDriver(new Uri("https://hub-cloud.browserstack.com/wd/hub/"), chromeCapability);

            return driver;


            //We use the Chrome browser
            //var chromeDriver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

            //return chromeDriver;
        }

        public void MarkResult(bool success, string reason)
        {
            var jsScript = "browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"" + (success ? "passed" : "failed") + "\", \"reason\": \"" + reason + "\"}}";

            ((IJavaScriptExecutor)Current).ExecuteScript(jsScript);
        }

        /// <summary>
        /// Disposes the Selenium web driver (closing the browser) after the Scenario completed
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
            {
                return;
            }

            if (_currentWebDriverLazy.IsValueCreated)
            {
                Current.Quit();
            }

            _isDisposed = true;
        }
    }
}
