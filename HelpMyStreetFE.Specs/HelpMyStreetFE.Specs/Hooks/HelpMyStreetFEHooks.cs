using HelpMyStreetFE.Specs.Drivers;
using HelpMyStreetFE.Specs.PageObjects;
using OpenQA.Selenium;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Hooks
{
    [Binding]
    public class HelpMyStreetFEHooks
    {
        [BeforeScenario("StartVolunteerBrowser")]
        public static void BeforeScenario_StartVolunteerBrowser(BrowserDriver browserDriver)
        {
            var pageObject = new GenericPageObject(browserDriver.VolunteerWebDriver);
            pageObject.EnsureHomePageIsOpenAndReset();
        }

        [BeforeScenario("StartAdminBrowser")]
        public static void BeforeScenario_StartAdminBrowser(BrowserDriver browserDriver)
        {
            var pageObject = new GenericPageObject(browserDriver.AdminWebDriver);
            pageObject.EnsureHomePageIsOpenAndReset();
        }

        [BeforeScenario("AcceptAllCookies")]
        public static void BeforeScenario_AcceptAllCookies(BrowserDriver browserDriver)
        {
            if (browserDriver.AdminWebDriverIsCreated)
            {
                AcceptAllCookies(browserDriver.AdminWebDriver);
            }
            if (browserDriver.VolunteerWebDriverIsCreated)
            {
                AcceptAllCookies(browserDriver.VolunteerWebDriver);
            }
        }

        private static void AcceptAllCookies(IWebDriver driver)
        {
            var pageObject = new GenericPageObject(driver);

            if (pageObject.IsVisible("gdpr-cookie-message-outer", null))
            {
                pageObject.Click("gdpr-cookie-accept", null);
                pageObject.WaitForDisplayedFalse("gdpr-cookie-message-outer", null);
            }
        }

        [BeforeScenario("MaximiseWindows")]
        public static void BeforeScenario_MaximiseWindow(BrowserDriver browserDriver)
        {
            if (browserDriver.AdminWebDriverIsCreated)
            {
                browserDriver.AdminWebDriver.Manage().Window.Maximize();
            }
            if (browserDriver.VolunteerWebDriverIsCreated)
            {
                browserDriver.VolunteerWebDriver.Manage().Window.Maximize();
            }
        }
    }
}
