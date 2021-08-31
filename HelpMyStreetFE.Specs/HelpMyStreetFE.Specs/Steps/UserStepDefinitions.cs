using System;
using System.Collections.Generic;
using System.Text;
using HelpMyStreetFE.Specs.Context;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Steps
{
    [Binding]
    public sealed class UserStepDefinitions
    {
        private readonly UserContext _userContext;
        private readonly LowLevelStepDefinitions _lowLevelStepDefinitions;

        public UserStepDefinitions(LowLevelStepDefinitions lowLevelStepDefinitions)
        {
            _lowLevelStepDefinitions = lowLevelStepDefinitions;

            _userContext = new UserContext();
        }

        [Given("the (.*) element (.*) has a new email address")]
        public void Given_the_USER_element_SELECTOR_has_a_new_email_address(string user, string selector)
        {

            _lowLevelStepDefinitions.Given_the_USER_element_SELECTOR_has_value_VALUE(user, selector, _userContext.Email);
        }
    }
}
