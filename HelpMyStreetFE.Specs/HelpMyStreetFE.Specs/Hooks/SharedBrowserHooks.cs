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
        private readonly ScenarioContext _scenarioContext;
        private readonly BrowserDriver _browserDriver;

        public SharedBrowserHooks(ScenarioContext scenarioContext, BrowserDriver browserDriver)
        {
            _scenarioContext = scenarioContext;
            _browserDriver = browserDriver;
        }

        [BeforeTestRun]
        public static void BeforeTestRun(ScenarioContext scenarioContext)
        {
           // scenarioContext.Add("test-run-id", Guid.NewGuid());
            //Initialize a shared BrowserDriver in the global container
            //testThreadContainer.BaseContainer.Resolve<BrowserDriver>();
        }

        [AfterScenario]
        public void AfterTestRun()
        {
            ScenarioExecutionStatus status = _scenarioContext.ScenarioExecutionStatus;

            switch (status)
            {
                case ScenarioExecutionStatus.OK:
                    _browserDriver.MarkResult(true, null);
                    break;
                default:
                    _browserDriver.MarkResult(false, _scenarioContext.TestError.Message);
                    break;
            }
        }
    }
}
