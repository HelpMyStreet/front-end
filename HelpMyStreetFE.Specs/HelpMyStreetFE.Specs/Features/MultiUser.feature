@StartVolunteerBrowser
@AcceptAllCookies
@StartAdminBrowser
@Browser_Chrome
@Browser_Edge
@Browser_Firefox
@Browser_Safari
Feature: MultiUserTest
	Test with two browser sessions

Scenario: Home page title
	Then the volunteer's page title should be Home Page - Help My Street
	And the admin's page title should be Home Page - Help My Street
