@StartVolunteerBrowser
@AcceptAllCookies
@StartAdminBrowser
Feature: MultiUserTest
	Test with two browser sessions

Scenario: Home page title
	Then the volunteer's page title should be Home Page - Help My Street
	And the admin's page title should be Home Page - Help My Street
