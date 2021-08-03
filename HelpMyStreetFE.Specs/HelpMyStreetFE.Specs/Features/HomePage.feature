Feature: HomePage

@StartAtHomePage
Scenario: Cookie banner displayed
	Then the element #gdpr-cookie-message-outer should be visible
	And the element #login-submit should not be clickable

@StartAtHomePage
Scenario: All cookies accepted
	When the element #gdpr-cookie-accept is clicked
	Then the element #gdpr-cookie-message-outer should not be visible

@StartAtHomePage
Scenario: Header login failure
	Given the element #email has value incorrect@nowhere.com
	And the element #password has value abcd1234
	When the element #login-submit is clicked
	Then the url should be login?email=incorrect@nowhere.com&er=login&ReturnUrl=null
	And the element #email should have value incorrect@nowhere.com
	And the element #password should be blank
	And the element #login-fail-message should be visible
	And the element #login-fail-message should have text Sorry, we couldn't find an account with that email address and password. Please check and try again

	
@StartAtHomePage
Scenario: Header blank login
	When the element #login-submit is clicked
	Then the url should be login?email=&er=email&ReturnUrl=null
	And the element #email should be blank
	And the element #password should be blank
	And the element #login-fail-message should not be visible
	And the element selected by input[name="email"]~.login__fail-message-main should be visible
	And the element selected by input[name="email"]~.login__fail-message-main should have text Please enter a valid email address