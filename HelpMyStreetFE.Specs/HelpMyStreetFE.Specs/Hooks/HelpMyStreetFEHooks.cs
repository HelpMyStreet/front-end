using HelpMyStreetFE.Specs.Drivers;
using HelpMyStreetFE.Specs.PageObjects;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Hooks
{
    [Binding]
    public class HelpMyStreetFEHooks
    {
        [BeforeScenario("StartAtHomePage")]
        public static void BeforeScenario(BrowserDriver browserDriver)
        {
            var calculatorPageObject = new GenericPageObject(browserDriver.Current);
            calculatorPageObject.EnsureHomePageIsOpenAndReset();
        }

        [BeforeScenario("AcceptAllCookies")]
        public static void BeforeScenario_AcceptAllCookies(BrowserDriver browserDriver)
        {
            var pageObject = new GenericPageObject(browserDriver.Current);

            if (pageObject.IsVisible("gdpr-cookie-message-outer", null))
            {
                pageObject.Click("gdpr-cookie-accept", null);
                pageObject.WaitForDisplayedFalse("gdpr-cookie-message-outer", null);
            }
        }
    }
}
