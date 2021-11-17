﻿using System.Drawing;
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
            pageObject.WaitForDisplayedTrue("#gdpr-cookie-accept");
            pageObject.Click("#gdpr-cookie-accept");
            pageObject.WaitForDisplayedFalse("#gdpr-cookie-message-outer");
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

        [BeforeScenario("800pxWidth")]
        public static void BeforeScenario_800pxWidth(BrowserDriver browserDriver)
        {
            if (browserDriver.AdminWebDriverIsCreated)
            {
                browserDriver.AdminWebDriver.Manage().Window.Size = new Size(800, 1000);
            }
            if (browserDriver.VolunteerWebDriverIsCreated)
            {
                browserDriver.VolunteerWebDriver.Manage().Window.Size = new Size(800, 1000);
            }
        }
    }
}