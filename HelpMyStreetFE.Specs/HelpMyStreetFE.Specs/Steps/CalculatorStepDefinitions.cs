using FluentAssertions;
using HelpMyStreetFE.Specs.Drivers;
using HelpMyStreetFE.Specs.PageObjects;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Steps
{
    [Binding]
    public sealed class CalculatorStepDefinitions
    {

        // For additional details on SpecFlow step definitions see https://go.specflow.org/doc-stepdef

        //private readonly ScenarioContext _scenarioContext;

        private readonly HomePageObject _homePageObject;

        public CalculatorStepDefinitions(BrowserDriver browserDriver)
        {
            //_scenarioContext = scenarioContext;
            _homePageObject = new HomePageObject(browserDriver.Current);
        }

        [Given("the username is (.*)")]
        public void GivenTheUsernameIs(string username)
        {
            //TODO: implement arrange (precondition) logic
            // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
            // To use the multiline text or the table argument of the scenario,
            // additional string/Table parameters can be defined on the step definition
            // method. 

            _homePageObject.EnterEmailAddress(username);

            //_scenarioContext.Pending();
        }

        [Given("the password is (.*)")]
        public void GivenThePasswordrIs(string password)
        {
            //TODO: implement arrange (precondition) logic
            // For storing and retrieving scenario-specific data see https://go.specflow.org/doc-sharingdata
            // To use the multiline text or the table argument of the scenario,
            // additional string/Table parameters can be defined on the step definition
            // method. 

            _homePageObject.EnterPassword(password);

            //_scenarioContext.Pending();
        }

        [When("the login form is submitted")]
        public void WhenTheLoginFormIsSubmitted()
        {
            //TODO: implement act (action) logic

            _homePageObject.ClickLogin();

            //_scenarioContext.Pending();
        }

        [Then("the result should be (.*)")]
        public void ThenTheResultShouldBe(string result)
        {
            //TODO: implement assert (verification) logic

            var actualResult = _homePageObject.WaitForNonEmptyResult();

            actualResult.Should().Be(result);

            //_scenarioContext.Pending();
        }
    }
}
