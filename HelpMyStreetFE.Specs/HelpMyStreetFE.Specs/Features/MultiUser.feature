@StartVolunteerBrowser
@AcceptAllCookies
@StartAdminBrowser
Feature: MultiUserTest
	Test with two browser sessions

Scenario: Home page title
	Then the page title should be Home Page - Help My Street
	And the secondary page title should be Home Page - Help My Street
