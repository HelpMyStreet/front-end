using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Drivers
{
    public class BrowserSeleniumDriverFactory
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;


        public BrowserSeleniumDriverFactory(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }

        public IWebDriver GetForBrowser(string browserId)
        {
            string lowerBrowserId = browserId.ToUpper();
            switch (lowerBrowserId)
            {
                case "EDGE": return GetEdgeDriver();
                case "CHROME": return GetChromeDriver();
                case "FIREFOX": return GetFirefoxDriver();
                case "SAFARI": return GetSafariDriver();
                case string browser: throw new NotSupportedException($"{browser} is not a supported browser");
                default: throw new NotSupportedException("not supported browser: <null>");
            }
        }

        private IWebDriver GetFirefoxDriver()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            FirefoxOptions firefoxCapability = new FirefoxOptions();
            firefoxCapability.AddAdditionalCapability("os_version", "10", true);
            firefoxCapability.AddAdditionalCapability("browser", "firefox", true);
            firefoxCapability.AddAdditionalCapability("browser_version", "latest", true);
            firefoxCapability.AddAdditionalCapability("os", "Windows", true);
            firefoxCapability.AddAdditionalCapability("name", _featureContext.FeatureInfo.Title + " / " + _scenarioContext.ScenarioInfo.Title, true); // test name
            firefoxCapability.AddAdditionalCapability("build", $"Version {version} on {System.Environment.MachineName}", true); // Your tests will be organized within this build
            firefoxCapability.AddAdditionalCapability("browserstack.user", Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME"), true);
            firefoxCapability.AddAdditionalCapability("browserstack.key", Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY"), true);

            IWebDriver driver = new RemoteWebDriver(new Uri("https://hub-cloud.browserstack.com/wd/hub/"), firefoxCapability);

            return driver;
        }

        private IWebDriver GetChromeDriver()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            ChromeOptions chromeCapability = new ChromeOptions();
            chromeCapability.AddAdditionalCapability("os_version", "10", true);
            chromeCapability.AddAdditionalCapability("browser", "Chrome", true);
            chromeCapability.AddAdditionalCapability("browser_version", "latest", true);
            chromeCapability.AddAdditionalCapability("os", "Windows", true);
            chromeCapability.AddAdditionalCapability("resolution", "1920x1080", true);
            chromeCapability.AddAdditionalCapability("project", "HelpMyStreetFE", true);
            chromeCapability.AddAdditionalCapability("name", _featureContext.FeatureInfo.Title + " / " + _scenarioContext.ScenarioInfo.Title, true);
            chromeCapability.AddAdditionalCapability("build", $"Version {version} on {System.Environment.MachineName}", true);
            chromeCapability.AddAdditionalCapability("browserstack.user", Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME"), true);
            chromeCapability.AddAdditionalCapability("browserstack.key", Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY"), true);

            IWebDriver driver = new RemoteWebDriver(new Uri("https://hub-cloud.browserstack.com/wd/hub/"), chromeCapability);

            return driver;
        }

        private IWebDriver GetEdgeDriver()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            EdgeOptions edgeCapability = new EdgeOptions();
            edgeCapability.AddAdditionalCapability("os_version", "10");
            edgeCapability.AddAdditionalCapability("browser", "Edge");
            edgeCapability.AddAdditionalCapability("browser_version", "latest");
            edgeCapability.AddAdditionalCapability("os", "Windows");
            edgeCapability.AddAdditionalCapability("resolution", "1920x1080");
            edgeCapability.AddAdditionalCapability("project", "HelpMyStreetFE");
            edgeCapability.AddAdditionalCapability("name", _featureContext.FeatureInfo.Title + " / " + _scenarioContext.ScenarioInfo.Title);
            edgeCapability.AddAdditionalCapability("build", $"Version {version} on {System.Environment.MachineName}");
            edgeCapability.AddAdditionalCapability("browserstack.user", Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME"));
            edgeCapability.AddAdditionalCapability("browserstack.key", Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY"));

            IWebDriver driver = new RemoteWebDriver(new Uri("https://hub-cloud.browserstack.com/wd/hub/"), edgeCapability);

            return driver;
        }

        private IWebDriver GetSafariDriver()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;

            SafariOptions safariCapability = new SafariOptions();
            safariCapability.AddAdditionalCapability("os_version", "Big Sur");
            safariCapability.AddAdditionalCapability("browser", "Safari");
            safariCapability.AddAdditionalCapability("browser_version", "latest");
            safariCapability.AddAdditionalCapability("os", "OS X");
            safariCapability.AddAdditionalCapability("resolution", "1920x1080");
            safariCapability.AddAdditionalCapability("project", "HelpMyStreetFE");
            safariCapability.AddAdditionalCapability("name", _featureContext.FeatureInfo.Title + " / " + _scenarioContext.ScenarioInfo.Title);
            safariCapability.AddAdditionalCapability("build", $"Version {version} on {System.Environment.MachineName}");
            safariCapability.AddAdditionalCapability("browserstack.user", Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME"));
            safariCapability.AddAdditionalCapability("browserstack.key", Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY"));

            IWebDriver driver = new RemoteWebDriver(new Uri("https://hub-cloud.browserstack.com/wd/hub/"), safariCapability);

            return driver;
        }
    }
}
