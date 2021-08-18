﻿@StartVolunteerBrowser
@AcceptAllCookies
Feature: SiteNavigation
	Navigation using top menu bar on home page

@MaximiseWindows
Scenario Outline: Top level navigation (wide screen)
	Given the volunteer's element selected by #site-nav ul li:nth-child(<index>) a should have text <Name>
	When the volunteer clicks the element selected by #site-nav ul li:nth-child(<index>) a
	Then the volunteer's url should be <url>
	And the volunteer's page title should be <Page Title>

	Examples: 
		| Name			| index | url				| Page Title										|
		| Request Help	|     1 | request-help/		| Request Help - Help My Street						|
		| Case Studies	|     2 | case-studies		| Case Studies - Help My Street						|
		| Resources		|     3 | resources			| Resources - Help My Street						|
		| Questions		|     4 | questions			| Frequently Asked Questions - Help My Street		|

@MaximiseWindows
Scenario: No further navigation links (wide screen)
	Then the volunteer's element selected by #site-nav ul li:nth-child(5) a should not be visible


Scenario Outline: Top level navigation (narrow screen)
	Given the volunteer has clicked the element #site-nav-toggle
	And the volunteer's element selected by #sitenavCollapsed #site-nav ul li:nth-child(<index>) a should have text <Name>
	When the volunteer clicks the element selected by #sitenavCollapsed #site-nav ul li:nth-child(<index>) a
	Then the volunteer's url should be <url>
	And the volunteer's page title should be <Page Title>

	Examples: 
		| Name			| index | url				| Page Title										|
		| Request Help	|     1 | request-help/		| Request Help - Help My Street						|
		| Case Studies	|     2 | case-studies		| Case Studies - Help My Street						|
		| Resources		|     3 | resources			| Resources - Help My Street						|
		| Questions		|     4 | questions			| Frequently Asked Questions - Help My Street		|

Scenario: No further navigation links (narrow screen)
	Given the volunteer has clicked the element #site-nav-toggle
	Then the volunteer's element selected by #sitenavCollapsed #site-nav ul li:nth-child(5) a should not be visible