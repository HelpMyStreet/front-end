using System;
using FluentAssertions;
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

        public GenericStepDefinitions(BrowserDriver browserDriver)
        {
            _browserDriver = browserDriver;
            _adminPageObjectLazy = new Lazy<GenericPageObject>(() => { return new GenericPageObject(_browserDriver.AdminWebDriver); });
            _volunuteerPageObjectLazy = new Lazy<GenericPageObject>(() => { return new GenericPageObject(_browserDriver.VolunteerWebDriver); });
        }

        [Given("the (.*) url is (.*)")]
        public void GivenTheUrlIs(string user, string value)
        {
            var pageObject = GetPageObject(user);
            pageObject.SetUrl(value);
        }

        [Given("the (.*) element #(.*) has value (.*)")]
        public void GivenTheElementWithIdHasValue(string user, string elementId, string value)
        {
            var pageObject = GetPageObject(user);
            pageObject.SetValue(elementId, null, value);
        }

        [Given("the (.*) element selected by (.*) has value (.*)")]
        public void GivenTheElementWithSelectorHasValue(string user, string selector, string value)
        {
            var pageObject = GetPageObject(user);
            pageObject.SetValue(null, selector, value);
        }

        [Given("the (.*) has clicked the element #(.*)")]
        public void GivenTheElementWithIdIsClicked(string user, string elementId)
        {
            var pageObject = GetPageObject(user);
            pageObject.Click(elementId, null);
        }

        [Given("the (.*) has clicked the element selected by (.*)")]
        public void GivenTheElementWithSelectorIsClicked(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.Click(null, selector);
        }

        [When("the (.*) clicks the element #(.*)")]
        public void WhenTheElementWithIdIsClicked(string user, string elementId)
        {
            var pageObject = GetPageObject(user);
            pageObject.Click(elementId, null);
        }

        [When("the (.*) clicks the element selected by (.*)")]
        public void WhenTheElementWithSelectorIsClicked(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.Click(null, selector);
        }

        [Then("the (.*) element selected by (.*) should have id #(.*)")]
        public void ThenTheElementWithSelectorShouldHaveId(string user, string selector, string expectedId)
        {
            var pageObject = GetPageObject(user);
            pageObject.GetId(selector).Should().Be(expectedId);
        }

        [Then("the (.*) element #(.*) should not be clickable")]
        public void ThenTheElementWithIdShouldNotBeClickable(string user, string elementId)
        {
            var pageObject = GetPageObject(user);
            pageObject.IsClickable(elementId, null).Should().BeFalse();
        }

        [Then("the (.*) element selected by (.*) should not be clickable")]
        public void ThenTheElementWithSelectorShouldNotBeClickable(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.IsClickable(null, selector).Should().BeFalse();
        }

        [Then("the (.*) element #(.*) should be visible")]
        public void ThenTheElementWithIdShouldBeVisible(string user, string elementId)
        {
            var pageObject = GetPageObject(user);
            pageObject.IsVisible(elementId, null).Should().BeTrue();
        }

        [Then("the (.*) element selected by (.*) should be visible")]
        public void ThenTheElementWithSelectorShouldBeVisible(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.IsVisible(null, selector).Should().BeTrue();
        }

        [Then("the (.*) element #(.*) should not be visible")]
        public void ThenTheElementWithIdShouldNotBeVisible(string user, string elementId)
        {
            var pageObject = GetPageObject(user);
            pageObject.WaitForDisplayedFalse(elementId, null);
        }

        [StepDefinition("the (.*) element selected by (.*) should not be visible")]
        public void ThenTheElementWithSelectorShouldNotBeVisible(string user, string selector)
        {
            var pageObject = GetPageObject(user);
            pageObject.WaitForDisplayedFalse(null, selector);
        }

        [Then("the (.*) element #(.*) should be blank")]
        public void ThenTheElementWithIdShouldHaveValue(string user, string elementId)
        {
            ThenTheElementWithIdShouldHaveValue(user, elementId, "");
        }

        [Then("the (.*) element selected by (.*) should be blank")]
        public void ThenTheElementWithSelectorShouldHaveValue(string user, string selector)
        {
            ThenTheElementWithSelectorShouldHaveValue(user, selector, "");
        }

        [Then("the (.*) element #(.*) should have value (.*)")]
        public void ThenTheElementWithIdShouldHaveValue(string user, string elementId, string expectedValue)
        {
            var pageObject = GetPageObject(user);
            pageObject.GetValue(elementId, null).Should().Be(expectedValue);
        }

        [StepDefinition("the (.*) element with selector (.*) should have value (.*)")]
        public void ThenTheElementWithSelectorShouldHaveValue(string user, string selector, string expectedValue)
        {
            var pageObject = GetPageObject(user);
            pageObject.GetValue(null, selector).Should().Be(expectedValue);
        }

        [StepDefinition("the (.*) element #(.*) should have text (.*)")]
        public void ThenTheElementWithIdShouldHaveText(string user, string elementId, string expectedValue)
        {
            var pageObject = GetPageObject(user);
            pageObject.GetText(elementId, null).Should().Be(expectedValue);
        }

        [StepDefinition("the (.*) element selected by (.*) should have text (.*)")]
        public void ThenTheElementWithSelectorShouldHaveText(string user, string selector, string expectedValue)
        {
            var pageObject = GetPageObject(user);
            pageObject.GetText(null, selector).Should().Be(expectedValue);
        }

        [StepDefinition("if visible, the (.*) element #(.*) should have text (.*)")]
        public void ThenTheElementWithIdIfVisibleShouldHaveText(string user, string elementId, string expectedValue)
        {
            var pageObject = GetPageObject(user);
            if (pageObject.IsVisible(elementId, null))
            {
                pageObject.GetText(elementId, null).Should().Be(expectedValue);
            }
        }

        [StepDefinition("if visible, the (.*) element selected by (.*) should have text (.*)")]
        public void ThenTheElementWithSelectorIfVisibleShouldHaveText(string user, string selector, string expectedValue)
        {
            var pageObject = GetPageObject(user);
            if (pageObject.IsVisible(null, selector))
            {
                pageObject.GetText(null, selector).Should().Be(expectedValue);
            }
        }

        [Then("the (.*) url should be (.*)")]
        public void ThenTheUrlShouldBe(string user, string url)
        {
            var pageObject = GetPageObject(user);
            pageObject.WaitForUrlChange().Should().Be(GenericPageObject.HomePageUrl + url);
        }

        [Then("the (.*) page title should be (.*)")]
        public void ThenThePageTitleShouldBe(string user, string expectedTitle)
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
