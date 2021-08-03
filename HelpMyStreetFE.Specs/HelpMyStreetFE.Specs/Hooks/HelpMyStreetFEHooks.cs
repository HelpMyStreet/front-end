using HelpMyStreetFE.Specs.Drivers;
using HelpMyStreetFE.Specs.PageObjects;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Hooks
{
    /// <summary>
    /// Calculator related hooks
    /// </summary>
    [Binding]
    public class HelpMyStreetFEHooks
    {
        ///<summary>
        ///  Reset the calculator before each scenario tagged with "Calculator"
        /// </summary>
        [BeforeScenario("StartAtHomePage")]
        public static void BeforeScenario(BrowserDriver browserDriver)
        {
            var calculatorPageObject = new HomePageObject(browserDriver.Current);
            calculatorPageObject.EnsureHomePageIsOpenAndReset();
        }
    }
}
