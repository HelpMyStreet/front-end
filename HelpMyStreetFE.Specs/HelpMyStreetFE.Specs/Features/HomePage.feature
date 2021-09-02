@StartVolunteerBrowser
Feature: HomePage

Scenario: Cookie banner displayed
	Then the volunteer element #gdpr-cookie-message-outer should be visible
	And the volunteer element #login-submit should not be clickable

@AcceptAllCookies
Scenario: All cookies accepted
	Then the volunteer element #gdpr-cookie-message-outer should not be visible

Scenario: Home page title
	Then the volunteer's page title should be Home Page - Help My Street

@AcceptAllCookies
Scenario: Header login failure
	Given the volunteer's element #email has value incorrect@nowhere.com
	And the volunteer's element #password has value abcd1234
	When the volunteer clicks the element #login-submit
	Then the volunteer's url should be login?email=incorrect@nowhere.com&er=login&ReturnUrl=null
	And the volunteer's element #email should have value incorrect@nowhere.com
	And the volunteer's element #password should be blank
	And the volunteer's element #login-fail-message should be visible
#	And the volunteer's element #login-fail-message should have text Sorry, we couldn't find an account with that email address and password. Please check and try again
	
@AcceptAllCookies
Scenario: Header login blank email
	When the volunteer clicks the element #login-submit
	Then the volunteer's url should be login?email=&er=email&ReturnUrl=null
	And the volunteer's element #email should be blank
	And the volunteer's element #password should be blank
	And the volunteer's element #login-fail-message should not be visible
	And the volunteer's element input[name="email"]~.login__fail-message-main should be visible
	And the volunteer's element input[name="email"]~.login__fail-message-main should have text Please enter a valid email address
		
@AcceptAllCookies
Scenario: Header login blank password
	Given the volunteer's element #email has value somewhere@anywhere.com
	When the volunteer clicks the element #login-submit
	Then the volunteer's url should be login?email=somewhere@anywhere.com&er=password&ReturnUrl=null
	And the volunteer's element #email should have value somewhere@anywhere.com
	And the volunteer's element #password should be blank
	And the volunteer's element #login-fail-message should be visible
	And the volunteer's element #login-fail-message should have text Please enter a valid password
	And the volunteer's element input[name="email"]~.login__fail-message-main should not be visible
