using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace HelpMyStreetFE.Specs.PageObjects
{
    public class GenericPageObject
    {
        public const string HomePageUrl = "https://helpmystreet-uat.azurewebsites.net/";

        private readonly IWebDriver _webDriver;

        public const int DefaultWaitInSeconds = 10;

        public GenericPageObject(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        public void SetUrl (string tailUrl)
        {
            _webDriver.Url = HomePageUrl + tailUrl;
        }

        public string GetId(string selector)
        {
            var el = GetElement(selector);
            return el.GetAttribute("id");
        }

        public string GetValue(string selector)
        {
            var el = GetElement(selector);
            return el.GetAttribute("value");
        }

        public void SetValue(string selector, string value)
        {
            var el = GetElement(selector);
            el.Clear();
            el.SendKeys(value);
        }

        public string GetText(string selector)
        {
            var el = GetElement(selector);
            return el.Text;
        }

        public void Click(string selector)
        {
            var el = GetElement(selector);
            ScrollTo(el);
            el.Click();
        }

        public bool IsClickable(string selector)
        {
            bool clickable = true;
            try
            {
                var el = GetElement(selector);
                el.Click();
            }
            catch (ElementClickInterceptedException)
            {
                clickable = false;
            }
            return clickable;
        }

        public bool IsVisible(string selector)
        {
            try
            {
                var el = GetElement(selector);
                ScrollTo(el);
                return el.Displayed;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        public void WaitForDisplayedFalse(string selector)
        {
            IWebElement el;
            try
            {
                el = GetElement(selector);
            }
            catch (NoSuchElementException)
            {
                // Element not in DOM
                return;
            }

            try
            {
                WaitUntilBool(() => el.Displayed, false);
            }
            catch (StaleElementReferenceException)
            {
                // Element has been removed from page
            }
        }

        public string WaitForUrlChange()
        {
            var initialUrl = _webDriver.Url;
            try
            {
                return WaitUntil(() => _webDriver.Url, result => !result.Equals(initialUrl));
            }
            catch (WebDriverTimeoutException)
            {
                return initialUrl;
            }
        }

        public string GetTitle()
        {
            return _webDriver.Title;
        }

        public void EnsureHomePageIsOpenAndReset()
        {
            //Open the home page in the browser if not opened yet
            if (_webDriver.Url != HomePageUrl)
            {
                _webDriver.Url = HomePageUrl;
            }
            //Otherwise reset the calculator by clicking the reset button
            else
            {
            }
        }


        /// <summary>
        /// Helper method to wait until the expected result is available on the UI
        /// </summary>
        /// <typeparam name="T">The type of result to retrieve</typeparam>
        /// <param name="getResult">The function to poll the result from the UI</param>
        /// <param name="isResultAccepted">The function to decide if the polled result is accepted</param>
        /// <returns>An accepted result returned from the UI. If the UI does not return an accepted result within the timeout an exception is thrown.</returns>
        private T WaitUntil<T>(Func<T> getResult, Func<T, bool> isResultAccepted) where T : class
        {
            var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(DefaultWaitInSeconds));
            return wait.Until(driver =>
            {
                var result = getResult();
                if (!isResultAccepted(result))
                    return default;

                return result;
            });
        }

        private void WaitUntilBool(Func<bool> getResult, bool expectedResult)
        {
            var wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(DefaultWaitInSeconds));
            wait.Until(driver =>
            {
                var result = getResult();
                return result == expectedResult;
            });
        }

        private IWebElement GetElement(string selector)
        {
            return _webDriver.FindElement(By.CssSelector(selector));
        }

        private void ScrollTo(IWebElement el)
        {
            ((IJavaScriptExecutor)_webDriver).ExecuteScript("arguments[0].scrollIntoView(true);", el);
        }
    }
}
