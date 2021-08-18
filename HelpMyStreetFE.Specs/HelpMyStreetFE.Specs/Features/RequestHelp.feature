@StartVolunteerBrowser
@AcceptAllCookies
Feature: RequestHelp
	Request Help form

Background: 
	Given the url is request-help/

Scenario Outline: Correct activities visible on generic Request Help form
	Then the element selected by .sm4:nth-child(<index>) .tiles__tile should have id #task_<SupportActivityId>
	And the element selected by #task_<SupportActivityId> .tiles__tile__content__header should have text <Name>
	And the element selected by #task_<SupportActivityId> .tiles__tile__content__description should have text <Description>

	Examples: 
		| Name			| index | SupportActivityId | Description																			|
		| Shopping		| 	  1 |				  1 | Picking up groceries and other essentials (e.g. food, toiletries, household products) |
		| Face Covering	|	  2 |				 12 | Finding someone to provide washable fabric face coverings								|
		| Check In		|	  3 |				 10 | Checking that someone is OK															|
		| Prescriptions	| 	  4 |				  2 | Collecting prescriptions from a local pharmacy										|
		| Errands		|	  5 |				  3 | Running essential local errands (e.g. posting mail)									|
		| Prepared Meal	|	  6 |				  6 | Getting a hot / pre-prepared meal														|
		| Friendly Chat	|	  7 |				  7 | A friendly chat on the phone															|
		| Homework		|	  8 |				  9 | Remote support for children being home-schooled										|
		| Other			|	  9 |				 11 | Please tell us more below																|

Scenario: No excess activities visible on generic Request Help form
	Then the element selected by .sm4:nth-child(10) .tiles__tile should not be visible