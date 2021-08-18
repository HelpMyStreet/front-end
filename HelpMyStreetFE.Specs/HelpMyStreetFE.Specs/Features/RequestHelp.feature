@StartAtHomePage
@AcceptAllCookies
Feature: RequestHelp
	Request Help form

Background: 
	Given the url is request-help/

Scenario: Correct activities visible on generic Request Help form
	Then the element selected by .sm4:nth-child(1) .tiles__tile should have id #task_1
	And the element selected by #task_1 .tiles__tile__content__header should have text Shopping
	And the element selected by #task_1 .tiles__tile__content__description should have text Picking up groceries and other essentials (e.g. food, toiletries, household products)
	And the element selected by .sm4:nth-child(2) .tiles__tile should have id #task_12
	And the element selected by #task_12 .tiles__tile__content__header should have text Face Covering
	And the element selected by #task_12 .tiles__tile__content__description should have text Finding someone to provide washable fabric face coverings
	And the element selected by .sm4:nth-child(3) .tiles__tile should have id #task_10
	And the element selected by #task_10 .tiles__tile__content__header should have text Check In
	And the element selected by #task_10 .tiles__tile__content__description should have text Checking that someone is OK
	And the element selected by .sm4:nth-child(4) .tiles__tile should have id #task_2
	And the element selected by #task_2 .tiles__tile__content__header should have text Prescriptions
	And the element selected by #task_2 .tiles__tile__content__description should have text Collecting prescriptions from a local pharmacy
	And the element selected by .sm4:nth-child(5) .tiles__tile should have id #task_3
	And the element selected by #task_3 .tiles__tile__content__header should have text Errands
	And the element selected by #task_3 .tiles__tile__content__description should have text Running essential local errands (e.g. posting mail)
	And the element selected by .sm4:nth-child(6) .tiles__tile should have id #task_6
	And the element selected by #task_6 .tiles__tile__content__header should have text Prepared Meal
	And the element selected by #task_6 .tiles__tile__content__description should have text Getting a hot / pre-prepared meal
	And the element selected by .sm4:nth-child(7) .tiles__tile should have id #task_7
	And the element selected by #task_7 .tiles__tile__content__header should have text Friendly Chat
	And the element selected by #task_7 .tiles__tile__content__description should have text A friendly chat on the phone
	And the element selected by .sm4:nth-child(8) .tiles__tile should have id #task_9
	And the element selected by #task_9 .tiles__tile__content__header should have text Homework 
	And the element selected by #task_9 .tiles__tile__content__description should have text Remote support for children being home-schooled
	And the element selected by .sm4:nth-child(9) .tiles__tile should have id #task_11
	And the element selected by #task_11 .tiles__tile__content__header should have text Other
	And the element selected by #task_11 .tiles__tile__content__description should have text Please tell us more below

