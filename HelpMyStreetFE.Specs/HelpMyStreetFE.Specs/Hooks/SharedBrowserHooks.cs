using System;
using System.Reflection;
using BoDi;
using HelpMyStreetFE.Specs.Drivers;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Hooks
{
    /// <summary>
    /// Share the same browser window for all scenarios
    /// </summary>
    /// <remarks>
    /// This makes the sequential execution of scenarios faster (opening a new browser window each time would take more time)
    /// As a tradeoff:
    ///  - we cannot run the tests in parallel
    ///  - we have to "reset" the state of the browser before each scenario
    /// </remarks>
    [Binding]
    public class SharedBrowserHooks
    {
        private readonly FeatureContext _featureContext;
        private readonly ScenarioContext _scenarioContext;
        private readonly BrowserDriver _browserDriver;

        public SharedBrowserHooks(FeatureContext featureContext, ScenarioContext scenarioContext, BrowserDriver browserDriver)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
            _browserDriver = browserDriver;
        }

        [BeforeFeature]
        public static void BeforeFeature()
        {
        }

        [AfterScenario]
        public void AfterScenario()
        {
            ScenarioExecutionStatus status = _scenarioContext.ScenarioExecutionStatus;

            switch (status)
            {
                case ScenarioExecutionStatus.OK:
                    _browserDriver.MarkResult(true, null);
                    break;
                default:
                    _browserDriver.MarkResult(false, _scenarioContext.TestError?.Message);
                    break;
            }
        }
    }
}
