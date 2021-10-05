Feature: Get latest exchange rates from https://api.exchangeratesapi.io/latest endpoint
	In order to know how much selected currency is worth
	As an API consumer
	I want to obtain foreign exchange rates for this currency

@positive
Scenario: Request latest exchange rates for selected base currency
	Given I want to obtain the latest exchange rates for <baseCurrency>
	When I send a GET HTTP request
	Then I receive a successful HTTP response
	And it contains all the latest exchange rates
Examples:
	| baseCurrency |
	| EUR |
	| USD |
	| PLN |

@positive
Scenario: Request latest exchange rates for selected currencies
	Given I want to obtain the latest exchange rates for EUR
	And I want to receive rates only for <targetCurrencies>
	When I send a GET HTTP request
	Then I receive a successful HTTP response
	And it contains the latest selected exchange rates
Examples:
	| targetCurrencies	|
	| NZD				|
	| USD,CAD			|
	| PLN,CZK,HRK		|
	| CAD,HKD,ISK,PHP,DKK,HUF,CZK,AUD,RON,SEK,IDR,INR,BRL,RUB,HRK |

@negative
Scenario: Request exchange rates for multi-currency base
	Given I request the latest exchange rates for USD, CZK
	When I send a GET HTTP request
	Then I receive a HTTP response with error
	And it contains information that exchange rate is not supported

@negative
Scenario: Request exchange rates for unknown currency
	Given I request the latest exchange rates for <unsupportedBaseCurrency>
	When I send a GET HTTP request
	Then I receive a HTTP response with error
	And it contains information that exchange rate is not supported
Examples:
	| unsupportedBaseCurrency	|
	| BIF						|
	| COP						|
	| TEST						|
	
@negative
Scenario: Request unknown exchange rates for selected currency
	Given I request the latest exchange rates for EUR
	And I want to receive rates only for <unsupportedTargetCurrencies>
	When I send a GET HTTP request
	Then I receive a HTTP response with error
	And it contains information that the selected exchange rates are not supported
	# Tests are failing because, in my opinion, error message is not correct for this kind of behaviour
Examples:
	| unsupportedTargetCurrencies	|
	| BIF							|
	| ERR,TEST						|

# Test ideas:
# - (negative) check how web page handles random 'endpoints' (e.g. https://api.exchangeratesapi.io/test)
# - performance tests - measure loading of 