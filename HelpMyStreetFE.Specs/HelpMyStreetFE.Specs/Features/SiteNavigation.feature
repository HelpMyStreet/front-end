@StartAtHomePage
@AcceptAllCookies
@MaximiseWindow
Feature: SiteNavigation
	Navigation using top menu bar on home page

Scenario Outline: Correct navigation links visible
	Given the element selected by #site-nav ul li:nth-child(<index>) a should have text <Name>
	When the element selected by #site-nav ul li:nth-child(<index>) a is clicked
	Then the url should be <url>
	And the page title should be <Page Title>

	Examples: 
		| Name			| index | url				| Page Title										|
		| Request Help	|     1 | request-help/		| Request Help - Help My Street						|
		| Case Studies	|     2 | case-studies		| Case Studies - Help My Street						|
		| Resources		|     3 | resources			| Resources - Help My Street						|
		| Questions		|     4 | questions			| Frequently Asked Questions - Help My Street		|


Scenario: No further navigation links
	But the element selected by #site-nav ul li:nth-child(5) a should not be visible