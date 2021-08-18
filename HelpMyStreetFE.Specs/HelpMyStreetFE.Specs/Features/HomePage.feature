@StartVolunteerBrowser
Feature: HomePage

Scenario: Cookie banner displayed
	Then the element #gdpr-cookie-message-outer should be visible
	And the element #login-submit should not be clickable

Scenario: All cookies accepted
	When the element #gdpr-cookie-accept is clicked
	Then the element #gdpr-cookie-message-outer should not be visible

Scenario: Home page title
	Then the page title should be Home Page - Help My Street

@AcceptAllCookies
Scenario: Header login failure
	Given the element #email has value incorrect@nowhere.com
	And the element #password has value abcd1234
	When the element #login-submit is clicked
	Then the url should be login?email=incorrect@nowhere.com&er=login&ReturnUrl=null
	And the element #email should have value incorrect@nowhere.com
	And the element #password should be blank
	And the element #login-fail-message should be visible
	And the element #login-fail-message should have text Sorry, we couldn't find an account with that email address and password. Please check and try again
	
@AcceptAllCookies
Scenario: Header login blank email
	When the element #login-submit is clicked
	Then the url should be login?email=&er=email&ReturnUrl=null
	And the element #email should be blank
	And the element #password should be blank
	And the element #login-fail-message should not be visible
	And the element selected by input[name="email"]~.login__fail-message-main should be visible
	And the element selected by input[name="email"]~.login__fail-message-main should have text Please enter a valid email address
		
@AcceptAllCookies
Scenario: Header login blank password
	Given the element #email has value somewhere@anywhere.com
	When the element #login-submit is clicked
	Then the url should be login?email=somewhere@anywhere.com&er=password&ReturnUrl=null
	And the element #email should have value somewhere@anywhere.com
	And the element #password should be blank
	And the element #login-fail-message should be visible
	And the element #login-fail-message should have text Please enter a valid password
	And the element selected by input[name="email"]~.login__fail-message-main should not be visible
