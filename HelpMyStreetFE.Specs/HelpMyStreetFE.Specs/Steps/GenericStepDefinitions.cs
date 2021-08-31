using System;
using FluentAssertions;
using HelpMyStreetFE.Specs.Context;
using HelpMyStreetFE.Specs.Drivers;
using HelpMyStreetFE.Specs.PageObjects;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Steps
{
    [Binding]
    public sealed class GenericStepDefinitions
    {
        private readonly BrowserDriver _browserDriver;
        private readonly Lazy<GenericPageObject> _adminPageObjectLazy;
        private readonly Lazy<GenericPageObject> _volunuteerPageObjectLazy;

        private readonly UserContext _userContext;

        public GenericStepDefinitions(BrowserDriver browserDriver)
        {
            _browserDriver = browserDriver;
            _adminPageObjectLazy = new Lazy<GenericPageObject>(() => { return new GenericPageObject(_browserDriver.AdminWebDriver); });
            _volunuteerPageObjectLazy = new Lazy<GenericPageObject>(() => { return new GenericPageObject(_browserDriver.VolunteerWebDriver); });

            _userContext = new UserContext();
        }

        [Given("the (.*) url is (.*)")]
        public void Given_the_USER_url_is_URL(string user, string url)
        {
            var pageObject = GetPageObject(user);
            pageObject.SetUrl(url);
        }

        [Given("the (.*) element (.*) has value (.*)")]
        public void Given_the_USER_element_SELECTOR_has_value_VALUE(string user, string selector, string value)
        {
            var pageObject = GetPageObject(user);
            pageObject.SetValue(selector, value);
        }

        [Given("the (.*) element (.*) has a new email address")]
        public void Given_the_USER_element_SELECTOR_has_a_new_email_address(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.SetValue(selector, _userContext.Email);
        }

        [StepDefinition("the (.*) (?:clicks|has clicked) the element (.*)")]
        public void The_USER_clicks_the_element_SELECTOR(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.Click(selector);
        }

        [Then("the (.*) element (.*) should have id (.*)")]
        public void Then_the_USER_element_SELECTOR_should_have_id_EXPECTEDID(string user, string selector, string expectedId)
        {
            var pageObject = GetPageObject(user);
            pageObject.GetId(selector).Should().Be(expectedId);
        }

        [Then("the (.*) element (.*) should not be clickable")]
        public void Then_the_USER_element_SELECTOR_should_not_be_clickable(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.IsClickable(selector).Should().BeFalse();
        }

        [Then("the (.*) element (.*) should be visible")]
        public void Then_the_USER_element_SELECTOR_should_be_visible(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.IsVisible(selector).Should().BeTrue();
        }

        [Then("the (.*) element (.*) should not be visible")]
        public void Then_the_USER_element_SELECTOR_should_not_be_visible(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.WaitForDisplayedFalse(selector);
        }


        [Then("the (.*) element (.*) should be blank")]
        public void Then_the_USER_element_SELECTOR_should_be_blank(string user, string selector)
        {
            The_USER_element_SELECTOR_should_have_value_EXPECTEDVALUE(user, selector, "");
        }

        [Then("the (.*) element (.*) should have value (.*)")]
        public void The_USER_element_SELECTOR_should_have_value_EXPECTEDVALUE(string user, string selector, string expectedValue)
        {
            var pageObject = GetPageObject(user);
            pageObject.GetValue(selector).Should().Be(expectedValue);
        }

        [StepDefinition("the (.*) element (.*) should have text (.*)")]
        public void The_USER_element_SELECTOR_should_have_text_EXPECTEDTEXT(string user, string selector, string expectedText)
        {
            var pageObject = GetPageObject(user);
            pageObject.GetText(selector).Should().Be(expectedText);
        }

        [Then("if visible, the (.*) element (.*) should have text (.*)")]
        public void If_visible_the_USER_element_SELECTOR_should_have_text_EXPECTEDTEXT(string user, string selector, string expectedText)
        {
            var pageObject = GetPageObject(user);
            if (pageObject.IsVisible(selector))
            {
                pageObject.GetText(selector).Should().Be(expectedText);
            }
        }

        [Then("the (.*) url should be (.*)")]
        public void Then_the_USER_url_should_be_EXPECTEDURL(string user, string expectedUrl)
        {
            var pageObject = GetPageObject(user);
            pageObject.WaitForUrlChange().Should().Be($"{GenericPageObject.HomePageUrl}{expectedUrl}");
        }

        [Then("the (.*) page title should be (.*)")]
        public void Then_the_USER_page_title_should_be_EXPECTEDTITLE(string user, string expectedTitle)
        {
            var pageObject = GetPageObject(user);
            pageObject.GetTitle().Should().Be(expectedTitle);
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
