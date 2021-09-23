using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

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
            try
            {
                return _browserSeleniumDriverFactory.GetForBrowser(testBrowserId);
            }
            catch (WebDriverException ex)
            {
                if (ex.Message.Contains("All parallel tests are currently in use"))
                {
                    Random random = new Random();
                    Thread.Sleep(4000 + random.Next(4000));
                    return GetWebDriver();
                }
                else
                {
                    throw;
                }
            }
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
