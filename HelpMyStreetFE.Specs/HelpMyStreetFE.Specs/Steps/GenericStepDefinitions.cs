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

        private GenericPageObject VolunteerPageObject
        {
            get { return _volunuteerPageObjectLazy.Value; }
        }

        private GenericPageObject AdminPageObject
        {
            get { return _adminPageObjectLazy.Value; }
        }

        public GenericStepDefinitions(BrowserDriver browserDriver)
        {
            _browserDriver = browserDriver;
            _adminPageObjectLazy = new Lazy<GenericPageObject>(() => { return new GenericPageObject(_browserDriver.AdminWebDriver); });
            _volunuteerPageObjectLazy = new Lazy<GenericPageObject>(() => { return new GenericPageObject(_browserDriver.VolunteerWebDriver); });
        }

        [Given("the url is (.*)")]
        public void GivenTheUrlIs(string value)
        {
            VolunteerPageObject.SetUrl(value);
        }

        [Given("the secondary url is (.*)")]
        public void GivenTheSecondaryUrlIs(string value)
        {
            AdminPageObject.SetUrl(value);
        }

        [Given("the element #(.*) has value (.*)")]
        public void GivenTheElementWithIdHasValue(string elementId, string value)
        {
            VolunteerPageObject.SetValue(elementId, null, value);
        }

        [Given("the element selected by (.*) has value (.*)")]
        public void GivenTheElementWithSelectorHasValue(string selector, string value)
        {
            VolunteerPageObject.SetValue(null, selector, value);
        }

        [When("the element #(.*) is clicked")]
        public void WhenTheElementWithIdIsClicked(string elementId)
        {
            VolunteerPageObject.Click(elementId, null);
        }

        [When("the element selected by (.*) is clicked")]
        public void WhenTheElementWithSelectorIsClicked(string selector)
        {
            VolunteerPageObject.Click(null, selector);
        }

        [Then("the element selected by (.*) should have id #(.*)")]
        public void ThenTheElementWithSelectorShouldHaveId(string selector, string expectedId)
        {
            VolunteerPageObject.GetId(selector).Should().Be(expectedId);
        }

        [Then("the element #(.*) should not be clickable")]
        public void ThenTheElementWithIdShouldNotBeClickable(string elementId)
        {
            VolunteerPageObject.IsClickable(elementId, null).Should().BeFalse();
        }

        [Then("the element selected by (.*) should not be clickable")]
        public void ThenTheElementWithSelectorShouldNotBeClickable(string selector)
        {
            VolunteerPageObject.IsClickable(null, selector).Should().BeFalse();
        }

        [Then("the element #(.*) should be visible")]
        public void ThenTheElementWithIdShouldBeVisible(string elementId)
        {
            VolunteerPageObject.IsVisible(elementId, null).Should().BeTrue();
        }

        [Then("the element selected by (.*) should be visible")]
        public void ThenTheElementWithSelectorShouldBeVisible(string selector)
        {
            VolunteerPageObject.IsVisible(null, selector).Should().BeTrue();
        }

        [Then("the element #(.*) should not be visible")]
        public void ThenTheElementWithIdShouldNotBeVisible(string elementId)
        {
            VolunteerPageObject.WaitForDisplayedFalse(elementId, null);
        }

        [StepDefinition("the element selected by (.*) should not be visible")]
        public void ThenTheElementWithSelectorShouldNotBeVisible(string selector)
        {
            VolunteerPageObject.WaitForDisplayedFalse(null, selector);
        }

        [Then("the element #(.*) should be blank")]
        public void ThenTheElementWithIdShouldHaveValue(string elementId)
        {
            ThenTheElementWithIdShouldHaveValue(elementId, "");
        }

        [Then("the element selected by (.*) should be blank")]
        public void ThenTheElementWithSelectorShouldHaveValue(string selector)
        {
            ThenTheElementWithSelectorShouldHaveValue(selector, "");
        }

        [Then("the element #(.*) should have value (.*)")]
        public void ThenTheElementWithIdShouldHaveValue(string elementId, string expectedValue)
        {
            VolunteerPageObject.GetValue(elementId, null).Should().Be(expectedValue);
        }

        [StepDefinition("the element with selector (.*) should have value (.*)")]
        public void ThenTheElementWithSelectorShouldHaveValue(string selector, string expectedValue)
        {
            VolunteerPageObject.GetValue(null, selector).Should().Be(expectedValue);
        }

        [StepDefinition("the element #(.*) should have text (.*)")]
        public void ThenTheElementWithIdShouldHaveText(string elementId, string expectedValue)
        {
            VolunteerPageObject.GetText(elementId, null).Should().Be(expectedValue);
        }

        [StepDefinition("the element selected by (.*) should have text (.*)")]
        public void ThenTheElementWithSelectorShouldHaveText(string selector, string expectedValue)
        {
            VolunteerPageObject.GetText(null, selector).Should().Be(expectedValue);
        }

        [Then("the url should be (.*)")]
        public void ThenTheUrlShouldBe(string url)
        {
            VolunteerPageObject.WaitForUrlChange().Should().Be(GenericPageObject.HomePageUrl + url);
        }

        [Then("the page title should be (.*)")]
        public void ThenThePageTitleShouldBe(string expectedTitle)
        {
            VolunteerPageObject.GetTitle().Should().Be(expectedTitle);
        }

        [Then("the secondary page title should be (.*)")]
        public void ThenTheSecondaryPageTitleShouldBe(string expectedTitle)
        {
            AdminPageObject.GetTitle().Should().Be(expectedTitle);
        }
    }
}
