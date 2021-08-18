@StartAtHomePage
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
		| index | SupportActivityId | Name			| Description																			|
		|	  1 |				  1 | Shopping		| Picking up groceries and other essentials (e.g. food, toiletries, household products) |
		|	  2 |				 12 | Face Covering	| Finding someone to provide washable fabric face coverings								|
		|	  3 |				 10 | Check In		| Checking that someone is OK															|
		|	  4 |				  2 | Prescriptions	| Collecting prescriptions from a local pharmacy										|
		|	  5 |				  3 | Errands		| Running essential local errands (e.g. posting mail)									|
		|	  6 |				  6 | Prepared Meal	| Getting a hot / pre-prepared meal														|
		|	  7 |				  7 | Friendly Chat	| A friendly chat on the phone															|
		|	  8 |				  9 | Homework		| Remote support for children being home-schooled										|
		|	  9 |				 11 | Other			| Please tell us more below																|

