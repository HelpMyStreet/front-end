using FluentAssertions;
using HelpMyStreetFE.Specs.Drivers;
using HelpMyStreetFE.Specs.PageObjects;
using TechTalk.SpecFlow;

namespace HelpMyStreetFE.Specs.Steps
{
    [Binding]
    public sealed class GenericStepDefinitions
    {

        private readonly GenericPageObject _pageObject;

        public GenericStepDefinitions(BrowserDriver browserDriver)
        {
            _pageObject = new GenericPageObject(browserDriver.Current);
        }

        [Given("the element #(.*) has value (.*)")]
        public void GivenTheElementWithIdHasValue(string elementId, string value)
        {
            _pageObject.SetValue(elementId, null, value);
        }

        [Given("the element selected by (.*) has value (.*)")]
        public void GivenTheElementWithSelectorHasValue(string selector, string value)
        {
            _pageObject.SetValue(null, selector, value);
        }

        [When("the element #(.*) is clicked")]
        public void WhenTheElementWithIdIsClicked(string elementId)
        {
            _pageObject.Click(elementId, null);
        }

        [When("the element selected by (.*) is clicked")]
        public void WhenTheElementWithSelectorIsClicked(string selector)
        {
            _pageObject.Click(null, selector);
        }

        [Then("the element #(.*) should not be clickable")]
        public void ThenTheElementWithIdShouldNotBeClickable(string elementId)
        {
            _pageObject.IsClickable(elementId, null).Should().BeFalse();
        }

        [Then("the element selected by (.*) should not be clickable")]
        public void ThenTheElementWithSelectorShouldNotBeClickable(string selector)
        {
            _pageObject.IsClickable(null, selector).Should().BeFalse();
        }

        [Then("the element #(.*) should be visible")]
        public void ThenTheElementWithIdShouldBeVisible(string elementId)
        {
            _pageObject.IsVisible(elementId, null).Should().BeTrue();
        }

        [Then("the element selected by (.*) should be visible")]
        public void ThenTheElementWithSelectorShouldBeVisible(string selector)
        {
            _pageObject.IsVisible(null, selector).Should().BeTrue();
        }

        [Then("the element #(.*) should not be visible")]
        public void ThenTheElementWithIdShouldNotBeVisible(string elementId)
        {
            _pageObject.WaitForDisplayedFalse(elementId, null);
        }

        [Then("the element selected by (.*) should not be visible")]
        public void ThenTheElementWithSelectorShouldNotBeVisible(string selector)
        {
            _pageObject.WaitForDisplayedFalse(null, selector);
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
            _pageObject.GetValue(elementId, null).Should().Be(expectedValue);
        }

        [Then("the element with selector (.*) should have value (.*)")]
        public void ThenTheElementWithSelectorShouldHaveValue(string selector, string expectedValue)
        {
            _pageObject.GetValue(null, selector).Should().Be(expectedValue);
        }

        [Then("the element #(.*) should have text (.*)")]
        public void ThenTheElementWithIdShouldHaveText(string elementId, string expectedValue)
        {
            _pageObject.GetText(elementId, null).Should().Be(expectedValue);
        }

        [Then("the element selected by (.*) should have text (.*)")]
        public void ThenTheElementWithSelectorShouldHaveText(string selector, string expectedValue)
        {
            _pageObject.GetText(null, selector).Should().Be(expectedValue);
        }

        [Then("the url should be (.*)")]
        public void ThenTheUrlShouldBe(string url)
        {
            _pageObject.WaitForUrlChange().Should().Be(GenericPageObject.HomePageUrl + url);
        }
    }
}
