using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace HelpMyStreetFE.Specs.PageObjects
{
    public class HomePageObject
    {  
        //The URL of the calculator to be opened in the browser
        private const string HomePageUrl = "https://localhost:5001/";

        //The Selenium web driver to automate the browser
        private readonly IWebDriver _webDriver;

        //The default wait time in seconds for wait.Until
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
            //Clear text box
            HeaderLoginForm_Email.Clear();
            //Enter text
            HeaderLoginForm_Email.SendKeys(value);
        }

        public void EnterPassword(string value)
        {
            //Clear text box
            HeaderLoginForm_Password.Clear();
            //Enter text
            HeaderLoginForm_Password.SendKeys(value);
        }

        public void ClickLogin()
        {
            //Click the add button
            HeaderLoginForm_LogIn.Click();
        }

        public void EnsureCalculatorIsOpenAndReset()
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
                ResetButtonElement.Click();

                //Wait until the result is empty again
                WaitForEmptyResult();
            }
        }

        public string WaitForNonEmptyResult()
        {
            //Wait for the result to be not empty
            return WaitUntil(
                () => ResultElement.GetAttribute("value"),
                result => !string.IsNullOrEmpty(result));
        }

        public string WaitForEmptyResult()
        {
            //Wait for the result to be empty
            return WaitUntil(
                () => ResultElement.GetAttribute("value"),
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
    }
}
