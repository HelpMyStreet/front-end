@StartVolunteerBrowser
@AcceptAllCookies
Feature: Registration
	Registration flow

Background: 
	Given the volunteer's url is registration

Scenario Outline: Invalid email addresses
	Given the volunteer's element #email has value <Email>
	And the volunteer's element #password has value asd!"£123ASD
	And the volunteer's element #confirm-password has value asd!"£123ASD
	And the volunteer's has clicked the element selected by #privacy-and-terms+span
	When the volunteer clicks the element #submit_button
	Then the volunteer's url should be registration
	And if visible, the volunteer's element selected by #registration_form input:first-child span should have text <Validation error>

	Examples: 
		| Email       | Validation error                   |
		|             | Please enter an email address      |
		| abc         | Please enter a valid email address |
		| @.com       | Please enter a valid email address |
		| a@b.c       | Please enter a valid email address |
		| abc@def.com | The email address is already in use by another account. |

