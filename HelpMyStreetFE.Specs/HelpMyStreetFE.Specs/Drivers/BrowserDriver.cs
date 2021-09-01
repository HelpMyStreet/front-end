using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;

namespace HelpMyStreetFE.Specs.Drivers
{
    public class BrowserDriver : IDisposable
    {
        private readonly BrowserSeleniumDriverFactory _browserSeleniumDriverFactory;
        private readonly Lazy<IWebDriver> _adminWebDriverLazy;
        private readonly Lazy<IWebDriver> _volunteerWebDriverLazy;
        private readonly Lazy<WebDriverWait> _waitLazy;
        private readonly TimeSpan _waitDuration = TimeSpan.FromSeconds(10);
        private bool _isDisposed;

        public BrowserDriver(BrowserSeleniumDriverFactory browserSeleniumDriverFactory)
        {
            _browserSeleniumDriverFactory = browserSeleniumDriverFactory;            
            _adminWebDriverLazy = new Lazy<IWebDriver>(GetWebDriver);
            _volunteerWebDriverLazy = new Lazy<IWebDriver>(GetWebDriver);
            //_waitLazy = new Lazy<WebDriverWait>(GetWebDriverWait);
        }

        /// <summary>
        /// The Selenium IWebDriver instance
        /// </summary>
        public IWebDriver AdminWebDriver => _adminWebDriverLazy.Value;
        public IWebDriver VolunteerWebDriver => _volunteerWebDriverLazy.Value;

        public bool AdminWebDriverIsCreated
        {
            get { return _adminWebDriverLazy.IsValueCreated; }
        }
        public bool VolunteerWebDriverIsCreated
        {
            get { return _volunteerWebDriverLazy.IsValueCreated; }
        }



        //public WebDriverWait Wait => _waitLazy.Value;

        //private WebDriverWait GetWebDriverWait()
        //{
        //    return new WebDriverWait(Current, _waitDuration);
        //}

        private IWebDriver GetWebDriver()
        {
            string testBrowserId = Environment.GetEnvironmentVariable("Test_Browser");
            return _browserSeleniumDriverFactory.GetForBrowser(testBrowserId);
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

            if (_adminWebDriverLazy.IsValueCreated)
            {
                AdminWebDriver.Quit();
            }
            if (_volunteerWebDriverLazy.IsValueCreated)
            {
                VolunteerWebDriver.Quit();
            }

            _isDisposed = true;
        }

        public void MarkResult(bool success, string reason)
        {
            var excapedReason = "";// reason?.Replace("\"", "'");
            var jsScript = "browserstack_executor: {\"action\": \"setSessionStatus\", \"arguments\": {\"status\":\"" + (success ? "passed" : "failed") + "\", \"reason\": \"" + excapedReason + "\"}}";

            if (_adminWebDriverLazy.IsValueCreated)
            {
                ((IJavaScriptExecutor)AdminWebDriver).ExecuteScript(jsScript);
            }
            if (_volunteerWebDriverLazy.IsValueCreated)
            {
                ((IJavaScriptExecutor)VolunteerWebDriver).ExecuteScript(jsScript);
            }
        }
    }
}
