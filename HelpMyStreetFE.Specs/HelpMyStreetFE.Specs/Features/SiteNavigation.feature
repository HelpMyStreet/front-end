@StartVolunteerBrowser
@AcceptAllCookies
@Desktop
Feature: SiteNavigation
	Navigation using top menu bar on home page

@MaximiseWindows

Scenario Outline: Primary navigation (wide screen)
	Given the volunteer's element #site-nav ul li:nth-child(<index>) a should have text <Name>
	When the volunteer clicks the element #site-nav ul li:nth-child(<index>) a
	Then the volunteer's url should be <url>
	And the volunteer's page title should be <Page Title>

	Examples: 
		| Name			| index | url				| Page Title										|
		| Request Help	|     1 | request-help/		| Request Help - Help My Street						|
		| Case Studies	|     2 | case-studies		| Case Studies - Help My Street						|
		| Resources		|     3 | resources			| Resources - Help My Street						|
		| Questions		|     4 | questions			| Frequently Asked Questions - Help My Street		|

@MaximiseWindows
Scenario: No further primary navigation links (wide screen)
	Then the volunteer's element #site-nav ul li:nth-child(5) a should not be visible

@800pxWidth
Scenario Outline: Primary navigation (narrow screen)
	Given the volunteer has clicked the element #site-nav-toggle
	And the volunteer's element #sitenavCollapsed #site-nav ul li:nth-child(<index>) a should have text <Name>
	When the volunteer clicks the element #sitenavCollapsed #site-nav ul li:nth-child(<index>) a
	Then the volunteer's url should be <url>
	And the volunteer's page title should be <Page Title>

	Examples: 
		| Name			| index | url				| Page Title										|
		| Request Help	|     1 | request-help/		| Request Help - Help My Street						|
		| Case Studies	|     2 | case-studies		| Case Studies - Help My Street						|
		| Resources		|     3 | resources			| Resources - Help My Street						|
		| Questions		|     4 | questions			| Frequently Asked Questions - Help My Street		|

@800pxWidth
Scenario: No further primary navigation links (narrow screen)
	Given the volunteer has clicked the element #site-nav-toggle
	Then the volunteer's element #sitenavCollapsed #site-nav ul li:nth-child(5) a should not be visible


@MaximiseWindows
Scenario Outline: Footer navigation
	Given the volunteer's element footer a:nth-child(<index>) should have text <Name>
	When the volunteer clicks the element footer a:nth-child(<index>)
	Then the volunteer's url should be <url>
	And the volunteer's page title should be <Page Title>

	Examples: 
		| Name					| index | url				| Page Title								|
		| Privacy Policy		|     2 | privacy-policy	| Privacy Policy - Help My Street			|
		| Terms & Conditions	|     3 | terms-conditions	| Terms and Conditions - Help My Street		|
		| About Us				|     4 | about-us			| About Us - Help My Street					|
		| Contact Us			|     5 | contact-us		| Contact Us - Help My Street				|

@MaximiseWindows
Scenario: No further footer navigation links (narrow screen)
	Then the volunteer's element footer a:nth-child(6) should not be visible

