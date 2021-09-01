using System;
using HelpMyStreetFE.Specs.Context;
using HelpMyStreetFE.Specs.Drivers;
using HelpMyStreetFE.Specs.PageObjects;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Steps
{
    [Binding]
    public sealed class UserStepDefinitions
    {
        private readonly BrowserDriver _browserDriver;
        private readonly Lazy<GenericPageObject> _adminPageObjectLazy;
        private readonly Lazy<GenericPageObject> _volunuteerPageObjectLazy;

        private readonly UserContext _userContext;

        public UserStepDefinitions(BrowserDriver browserDriver)
        {
            _browserDriver = browserDriver;
            _adminPageObjectLazy = new Lazy<GenericPageObject>(() => { return new GenericPageObject(_browserDriver.AdminWebDriver); });
            _volunuteerPageObjectLazy = new Lazy<GenericPageObject>(() => { return new GenericPageObject(_browserDriver.VolunteerWebDriver); });

            _userContext = new UserContext();
        }

        [Given("the (.*) element (.*) has (?:a|the) unique email address")]
        public void Given_the_USER_element_SELECTOR_has_a_unique_email_address(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.SetValue(selector, _userContext.Email);
        }

        private GenericPageObject GetPageObject(string user)
        {
            return user switch
            {
                "volunteer" => _volunuteerPageObjectLazy.Value,
                "volunteer's" => _volunuteerPageObjectLazy.Value,
                "admin" => _adminPageObjectLazy.Value,
                "admin's" => _adminPageObjectLazy.Value,
                _ => throw new ArgumentException($"Unexpected user {user}", nameof(user))
            };
        }
    }
}
