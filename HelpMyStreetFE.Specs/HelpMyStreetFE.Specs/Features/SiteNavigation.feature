@StartAtHomePage
@AcceptAllCookies
@MaximiseWindow
Feature: SiteNavigation
	Navigation using top menu bar on home page

Scenario: Correct navigation links visible
	

Scenario: Navigate to Request Help
	Given the element selected by #site-nav ul li:nth-child(1) a should have text Request Help
	When the element selected by #site-nav ul li:nth-child(1) a is clicked
	Then the url should be request-help/
	And the page title should be Request Help - Help My Street

Scenario: Navigate to Case Studies
	Given the element selected by #site-nav ul li:nth-child(2) a should have text Case Studies
	When the element selected by #site-nav ul li:nth-child(2) a is clicked
	Then the url should be case-studies
	And the page title should be Case Studies - Help My Street

Scenario: Navigate to Resources
	Given the element selected by #site-nav ul li:nth-child(3) a should have text Resources
	When the element selected by #site-nav ul li:nth-child(3) a is clicked
	Then the url should be resources
	And the page title should be Resources - Help My Street

Scenario: Navigate to Questions
	Given the element selected by #site-nav ul li:nth-child(4) a should have text Questions
	When the element selected by #site-nav ul li:nth-child(4) a is clicked
	Then the url should be questions
	And the page title should be Frequently Asked Questions - Help My Street

Scenario: No further navigation links
	But the element selected by #site-nav ul li:nth-child(5) a should not be visible