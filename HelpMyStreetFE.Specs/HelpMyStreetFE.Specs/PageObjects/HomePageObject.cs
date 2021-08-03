using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace HelpMyStreetFE.Specs.PageObjects
{
    public class HomePageObject
    {
        private const string HomePageUrl = "https://localhost:5001/";

        private readonly IWebDriver _webDriver;

        public const int DefaultWaitInSeconds = 5;

        public HomePageObject(IWebDriver webDriver)
        {
            _webDriver = webDriver;
        }

        //Finding elements by ID
        private IWebElement HeaderLoginForm_Email => _webDriver.FindElement(By.Id("email"));
        private IWebElement HeaderLoginForm_Password => _webDriver.FindElement(By.Id("password"));
        private IWebElement HeaderLoginForm_LogIn => _webDriver.FindElement(By.Id("login-submit"));

        public void EnterEmailAddress(string value)
        {
            HeaderLoginForm_Email.Clear();
            HeaderLoginForm_Email.SendKeys(value);
        }

        public void EnterPassword(string value)
        {
            HeaderLoginForm_Password.Clear();
            HeaderLoginForm_Password.SendKeys(value);
        }

        public string GetValue(string elementId)
        {
            IWebElement el = _webDriver.FindElement(By.Id(elementId));
            return el.GetAttribute("value");
        }

        public string GetText(string selector)
        {
            IWebElement el = _webDriver.FindElement(By.CssSelector(selector));
            return el.Text;
        }

        public void ClickLogin()
        {
            HeaderLoginForm_LogIn.Click();
        }

        public void Click(string elementId)
        {
            IWebElement el = _webDriver.FindElement(By.Id(elementId));
            el.Click();
        }

        public bool IsClickable(string elementId)
        {
            bool clickable = true;
            try
            {
                IWebElement el = _webDriver.FindElement(By.Id(elementId));
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
            IWebElement el = _webDriver.FindElement(By.CssSelector(selector));
            return el.Displayed;
        }

        public void WaitForDisplayedFalse(string elementId, string selector)
        {
            IWebElement el;

            if (elementId != null)
            {
                el = _webDriver.FindElement(By.Id(elementId));
            }
            else
            {
                el = _webDriver.FindElement(By.CssSelector(selector));
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

        public void EnsureHomePageIsOpenAndReset()
        {
            //Open the calculator page in the browser if not opened yet
            if (_webDriver.Url != HomePageUrl)
            {
                _webDriver.Url = HomePageUrl;
            }
            //Otherwise reset the calculator by clicking the reset button
            else
            {
                //Click the rest button
                //ResetButtonElement.Click();

                //Wait until the result is empty again
                WaitForEmptyResult();
            }
        }

        public string WaitForNonEmptyResult()
        {
            return "";
            //Wait for the result to be not empty
            /*return WaitUntil(
                () => ResultElement.GetAttribute("value"),
                result => !string.IsNullOrEmpty(result));*/
        }

        public string WaitForEmptyResult()
        {
            //Wait for the result to be empty
            return WaitUntil(
                () => HeaderLoginForm_Email.GetAttribute("value"),
                result => result == string.Empty);
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

    }
}
