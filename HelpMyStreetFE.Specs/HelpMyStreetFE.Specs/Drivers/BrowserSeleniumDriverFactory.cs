using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Safari;
using System;
using System.Reflection;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Drivers
{
    struct BrowserDetails
    {
        private FeatureContext _featureContext;
        private ScenarioContext _scenarioContext;
        private Version _version;
        public BrowserDetails(FeatureContext featureContext, ScenarioContext scenarioContext,  Version version)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
            _version = version;
        }

        public string BrowserVersion { get { return "latest"; } }
        public string Resolution { get { return "1920x1080";} }
        public string Project { get { return "HelpMyStreetFE"; } }
        public string BrowserStackUser { get { return Environment.GetEnvironmentVariable("BROWSERSTACK_USERNAME"); } }
        public string BrowserStackAccessKey { get { return Environment.GetEnvironmentVariable("BROWSERSTACK_ACCESS_KEY"); } }
        public bool Debug { get { return true; } }
        public string Console { get { return "info"; } }
        public string NetworkLogs { get { return "true"; } }
        public string Name { get { return _featureContext.FeatureInfo.Title + " / " + _scenarioContext.ScenarioInfo.Title; } }
        public string Build { get { return $"Version {_version} on {Environment.MachineName}"; } }
    }

    struct Windows
    {
        public string OSVersion { get { return "10"; }}
        public string OS { get { return "Windows"; } }
    }

    struct MacOS
    {
        public string OSVersion { get { return "Big Sur"; } }
        public string OS { get { return "OS X"; } }
    }

    public class BrowserSeleniumDriverFactory
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;
        private Version _version;
        private BrowserDetails _browserDetails;
        private Windows _windows;
        private MacOS _macOS;
        private const string URI = "https://hub-cloud.browserstack.com/wd/hub/";

        public BrowserSeleniumDriverFactory(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
            _version = Assembly.GetExecutingAssembly().GetName().Version;
            _browserDetails = new BrowserDetails(_featureContext, _scenarioContext,_version);
            _windows = new Windows();
            _macOS = new MacOS();
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
                case "ANDROID": return GetAndroidDriver();
                case "IOS": return GetIOSDriver();
                case string browser: throw new NotSupportedException($"{browser} is not a supported browser");
                default: throw new NotSupportedException("not supported browser: <null>");
            }
        }


        private IWebDriver GetFirefoxDriver()
        {
            FirefoxOptions firefoxCapability = new FirefoxOptions();
            firefoxCapability.AddAdditionalCapability("browser", "firefox", true);
            firefoxCapability.AddAdditionalCapability("os_version", _windows.OSVersion, true);            
            firefoxCapability.AddAdditionalCapability("browser_version", _browserDetails.BrowserVersion, true);
            firefoxCapability.AddAdditionalCapability("os", _windows.OS, true);
            firefoxCapability.AddAdditionalCapability("resolution", _browserDetails.Resolution, true);
            firefoxCapability.AddAdditionalCapability("project", _browserDetails.Project, true);
            firefoxCapability.AddAdditionalCapability("name", _browserDetails.Name, true); // test name
            firefoxCapability.AddAdditionalCapability("build", _browserDetails.Build, true); // Your tests will be organized within this build
            firefoxCapability.AddAdditionalCapability("browserstack.user", _browserDetails.BrowserStackUser, true);
            firefoxCapability.AddAdditionalCapability("browserstack.key", _browserDetails.BrowserStackAccessKey, true);
            firefoxCapability.AddAdditionalCapability("browserstack.debug", _browserDetails.Debug, true);
            firefoxCapability.AddAdditionalCapability("browserstack.console", _browserDetails.Console, true);
            firefoxCapability.AddAdditionalCapability("browserstack.networkLogs", _browserDetails.NetworkLogs, true);

            IWebDriver driver = new RemoteWebDriver(new Uri(URI), firefoxCapability);

            return driver;
        }

        private IWebDriver GetChromeDriver()
        {
            ChromeOptions chromeCapability = new ChromeOptions();
            chromeCapability.AddAdditionalCapability("browser", "Chrome", true);
            chromeCapability.AddAdditionalCapability("os_version", _windows.OSVersion, true);            
            chromeCapability.AddAdditionalCapability("browser_version", _browserDetails.BrowserVersion, true);
            chromeCapability.AddAdditionalCapability("os", _windows.OS, true);
            chromeCapability.AddAdditionalCapability("resolution", _browserDetails.Resolution, true);
            chromeCapability.AddAdditionalCapability("project", _browserDetails.Project, true);
            chromeCapability.AddAdditionalCapability("name", _browserDetails.Name, true);
            chromeCapability.AddAdditionalCapability("build", _browserDetails.Build, true);
            chromeCapability.AddAdditionalCapability("browserstack.user", _browserDetails.BrowserStackUser, true);
            chromeCapability.AddAdditionalCapability("browserstack.key", _browserDetails.BrowserStackAccessKey, true);
            chromeCapability.AddAdditionalCapability("browserstack.debug", _browserDetails.Debug, true);
            chromeCapability.AddAdditionalCapability("browserstack.console", _browserDetails.Console, true);
            chromeCapability.AddAdditionalCapability("browserstack.networkLogs", _browserDetails.NetworkLogs, true);

            IWebDriver driver = new RemoteWebDriver(new Uri(URI), chromeCapability);

            return driver;
        }

        private IWebDriver GetEdgeDriver()
        {
            EdgeOptions edgeCapability = new EdgeOptions();
            edgeCapability.AddAdditionalCapability("browser", "Edge");
            edgeCapability.AddAdditionalCapability("os_version", _windows.OSVersion);
            edgeCapability.AddAdditionalCapability("browser_version", _browserDetails.BrowserVersion);
            edgeCapability.AddAdditionalCapability("os", _windows.OS);
            edgeCapability.AddAdditionalCapability("resolution", _browserDetails.Resolution);
            edgeCapability.AddAdditionalCapability("project", _browserDetails.Project);
            edgeCapability.AddAdditionalCapability("name", _browserDetails.Name);
            edgeCapability.AddAdditionalCapability("build", _browserDetails.Build);
            edgeCapability.AddAdditionalCapability("browserstack.user", _browserDetails.BrowserStackUser);
            edgeCapability.AddAdditionalCapability("browserstack.key", _browserDetails.BrowserStackAccessKey);
            edgeCapability.AddAdditionalCapability("browserstack.debug", _browserDetails.Debug);
            edgeCapability.AddAdditionalCapability("browserstack.console", _browserDetails.Console);
            edgeCapability.AddAdditionalCapability("browserstack.networkLogs", _browserDetails.NetworkLogs);

            IWebDriver driver = new RemoteWebDriver(new Uri(URI), edgeCapability);

            return driver;
        }

        private IWebDriver GetSafariDriver()
        {
            SafariOptions safariCapability = new SafariOptions();
            safariCapability.AddAdditionalCapability("browser", "Safari");
            safariCapability.AddAdditionalCapability("os_version", _macOS.OSVersion);            
            safariCapability.AddAdditionalCapability("browser_version", _browserDetails.BrowserVersion);
            safariCapability.AddAdditionalCapability("os", _macOS.OS);
            safariCapability.AddAdditionalCapability("resolution", _browserDetails.Resolution);
            safariCapability.AddAdditionalCapability("project", _browserDetails.Project);
            safariCapability.AddAdditionalCapability("name", _browserDetails.Name);
            safariCapability.AddAdditionalCapability("build", _browserDetails.Build);
            safariCapability.AddAdditionalCapability("browserstack.user", _browserDetails.BrowserStackUser);
            safariCapability.AddAdditionalCapability("browserstack.key", _browserDetails.BrowserStackAccessKey);
            safariCapability.AddAdditionalCapability("browserstack.debug", _browserDetails.Debug);
            safariCapability.AddAdditionalCapability("browserstack.console", _browserDetails.Console);
            safariCapability.AddAdditionalCapability("browserstack.networkLogs", _browserDetails.NetworkLogs);

            IWebDriver driver = new RemoteWebDriver(new Uri(URI), safariCapability);

            return driver;
        }

        private IWebDriver GetAndroidDriver()
        {
            ChromeOptions androidCapability = new ChromeOptions();
            androidCapability.AddAdditionalCapability("browser", "Android", true);
            androidCapability.AddAdditionalCapability("real_mobile", "true", true);
            androidCapability.AddAdditionalCapability("device", "Samsung Galaxy S20", true);
            androidCapability.AddAdditionalCapability("os_version", "10.0", true);
            androidCapability.AddAdditionalCapability("project", _browserDetails.Project, true);
            androidCapability.AddAdditionalCapability("name", _browserDetails.Name, true);
            androidCapability.AddAdditionalCapability("build", _browserDetails.Build, true);
            androidCapability.AddAdditionalCapability("browserstack.user", _browserDetails.BrowserStackUser, true);
            androidCapability.AddAdditionalCapability("browserstack.key", _browserDetails.BrowserStackAccessKey, true);
            androidCapability.AddAdditionalCapability("browserstack.debug", _browserDetails.Debug, true);
            androidCapability.AddAdditionalCapability("browserstack.console", _browserDetails.Console, true);
            androidCapability.AddAdditionalCapability("browserstack.networkLogs", _browserDetails.NetworkLogs, true);

            IWebDriver driver = new RemoteWebDriver(new Uri(URI), androidCapability);

            return driver;
        }

        private IWebDriver GetIOSDriver()
        {
            SafariOptions iosCapability = new SafariOptions();
            iosCapability.AddAdditionalCapability("browser", "iPhone");
            iosCapability.AddAdditionalCapability("real_mobile", "true");
            iosCapability.AddAdditionalCapability("device", "iPhone 12");
            iosCapability.AddAdditionalCapability("os_version", "14");            
            iosCapability.AddAdditionalCapability("project", _browserDetails.Project);
            iosCapability.AddAdditionalCapability("name", _browserDetails.Name);
            iosCapability.AddAdditionalCapability("build", _browserDetails.Build);
            iosCapability.AddAdditionalCapability("browserstack.user", _browserDetails.BrowserStackUser);
            iosCapability.AddAdditionalCapability("browserstack.key", _browserDetails.BrowserStackAccessKey);
            iosCapability.AddAdditionalCapability("browserstack.debug", _browserDetails.Debug);
            iosCapability.AddAdditionalCapability("browserstack.console", _browserDetails.Console);
            iosCapability.AddAdditionalCapability("browserstack.networkLogs", _browserDetails.NetworkLogs);

            IWebDriver driver = new RemoteWebDriver(new Uri(URI), iosCapability);

            return driver;
        }
    }
}
