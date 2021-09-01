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
	And the volunteer has clicked the element #privacy-and-terms+span
	When the volunteer clicks the element #submit_button
	Then the volunteer's url should be registration
	And if visible, the volunteer's element #registration_form .input:first-child span should have text <Validation error>

	Examples: 
		| Email       | Validation error                                        |
		|             | Please enter an email address                           |
		| abc         | Please enter a valid email address                      |
		| @.com       | Please enter a valid email address                      |
		| a@b.c       | Please enter a valid email address                      |

Scenario Outline: Invalid password
	Given the volunteer's element #email has a unique email address 
	And the volunteer's element #password has value <Password>
	And the volunteer's element #confirm-password has value <Password>
	And the volunteer has clicked the element #privacy-and-terms+span
	When the volunteer clicks the element #submit_button
	Then the volunteer's url should be registration
	And the volunteer's element #registration_form .input:nth-child(2) span.error should have text <Validation error>

	Examples: 
		| Password         | Validation error             |
		|                  | Please use a strong password |
		| a                | Please use a strong password |
		| aA1!bB2"         | Please use a strong password |
		| !"£$%QWERT123456 | Please use a strong password |
		| !"£$%qwert123456 | Please use a strong password |
		| !"£$%qwertQWERTY | Please use a strong password |

Scenario Outline: Invalid confirm password
	Given the volunteer's element #email has a unique email address 
	And the volunteer's element #password has value <Password>
	And the volunteer's element #confirm-password has value <Confirmation>
	And the volunteer has clicked the element #privacy-and-terms+span
	When the volunteer clicks the element #submit_button
	Then the volunteer's url should be registration
	And the volunteer's element #registration_form .input:nth-child(3) span.error should have text <Validation error>

	Examples: 
		| Password     | Confirmation    | Validation error              |
		| abcABC123!"£ | abcABC123!"£999 | Please ensure passwords match |


@StartAdminBrowser
@AcceptAllCookies
Scenario: Existing user
	Given the volunteer's element #email has a unique email address
	And the volunteer's element #password has value asd!"£123ASD
	And the volunteer's element #confirm-password has value asd!"£123ASD
	And the volunteer has clicked the element #privacy-and-terms+span
	And the volunteer has clicked the element #submit_button
	And the volunteer's url should be registration/step-two
	Given the admin's url is registration
	And the admin's element #email has the unique email address
	And the admin's element #password has value abcABC123!"£
	And the admin's element #confirm-password has value abcABC123!"£
	And the admin has clicked the element #privacy-and-terms+span
	When the admin clicks the element #submit_button
	Then the admin's url should be registration
	And the admin's element #registration_form .input:first-child span should be visible
	And the admin's element #registration_form .input:first-child span should have text The email address is already in use by another account.

@StartAdminBrowser
@AcceptAllCookies
Scenario: Resume registration at step 2
	Given the volunteer's element #email has a unique email address
	And the volunteer's element #password has value abcABC123!"£
	And the volunteer's element #confirm-password has value abcABC123!"£
	And the volunteer has clicked the element #privacy-and-terms+span
	And the volunteer has clicked the element #submit_button
	And the volunteer's url should be registration/step-two
	Given the admin's url is /login
	And the admin's element #email has the unique email address
	And the admin's element #password has value abcABC123!"£
	When the admin presses the element #login-submit
	Then the admin's url should be registration/step-two
