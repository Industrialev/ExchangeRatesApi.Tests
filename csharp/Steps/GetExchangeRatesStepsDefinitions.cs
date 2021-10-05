using System.Net.Http;
using System.Net.Http.Headers;
using TechTalk.SpecFlow;
using NUnit.Framework;
using Newtonsoft.Json;
using ExchangeRatesApi.Tests.Helpers;
using System.IO;
using System;
using System.Net;
using System.Web;
using System.Collections.Specialized;
using ExchangeRatesApi.Tests.Drivers;

namespace ExchangeRatesApi.Tests.Steps
{
    [Binding]
    public sealed class GetExchangeRatesStepsDefinitions
    {
        // Source: 'Framework for the euro foreign exchange reference rates' (chapter 2.5)
        private const int CountOfAllAvailableRates = 32;

        public Version ExpectedHttpVersion = new Version(1, 1);
        private string ExpectedBaseCurrency;

        private static HttpRequestDriver httpRequestDriver;

        private int expectedCurrenciesCount;
        private static string baseUri;

        [BeforeScenario]
        public static void InitializeHttpClient()
        {
            baseUri = "https://api.exchangeratesapi.io/latest";
            httpRequestDriver = new HttpRequestDriver(baseUri);
        }

        [AfterScenario]
        public static void Dispose()
        {
            httpRequestDriver.Dispose();
        }

        public GetExchangeRatesStepsDefinitions()
        {
            Console.WriteLine($"Tested endpoint: {baseUri}");
        }

        [Given("I want to obtain the latest exchange rates for (.*)")]
        [Given("I request the latest exchange rates for (.*)")]
        public void GivenIWantToObtainLatestExchangeRatesFor(string currency)
        {
            ExpectedBaseCurrency = currency;
            
            httpRequestDriver.AddElementToQuery(ApiParameters.Base, currency);
            expectedCurrenciesCount = GetCountOfAvailableRates(currency);
        }

        [Given("I want to receive rates only for (.*)")]
        public void GivenIWantToReceiveValuesOnlyForSpecific(string currencies)
        {
            expectedCurrenciesCount = GetCountOfSelectedCurrencies(currencies);
            httpRequestDriver.AddElementToQuery(ApiParameters.Symbols, currencies);
        }

        [When(@"I send a GET HTTP request")]
        public void WhenISendGetHttpRequest()
        {
            httpRequestDriver.SendGetRequest();
        }

        [Then("I receive a successful HTTP response")]
        public void ThenIReceiveASuccessfulHttpResponse()
        {
            ValidateHttpResponse(HttpStatusCode.OK);
        }

        [Then("I receive a HTTP response with error")]
        public void ThenIReceiveAValidHttpResponse()
        {
            ValidateHttpResponse(HttpStatusCode.BadRequest);
        }

        [Then("it contains all the latest exchange rates")]
        [Then("it contains the latest selected exchange rates")]
        public void ThenItContainsTheLatestSelectedExchangeRates()
        {
            var serializedResponse = httpRequestDriver.GetDeserializedResponse<ExchangeRates>();

            Assert.That(serializedResponse.Rates.Count, Is.EqualTo(expectedCurrenciesCount));
            Assert.That(serializedResponse.Date, Is.LessThan(DateTime.Now));
            Assert.That(serializedResponse.Base, Is.EqualTo(ExpectedBaseCurrency));
        }

        [Then("it contains information that exchange rate is not supported")]
        [Then("it contains information that the selected exchange rates are not supported")]
        public void ThenItContainsErrorInformationThatCurrencyIsNotSupported()
        {
            var serializedResponse = httpRequestDriver.GetDeserializedResponse<ErrorMessage>();
            Assert.That(serializedResponse.Content.Contains("is not supported"));
        }

        private int GetCountOfSelectedCurrencies(string currencies)
        {
            return currencies.Split(",", StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private void ValidateHttpResponse(HttpStatusCode expectedStatusCode)
        {
            Assert.That(httpRequestDriver.HttpResponse.StatusCode, Is.EqualTo(expectedStatusCode));
            Assert.That(httpRequestDriver.HttpResponse.Version, Is.EqualTo(ExpectedHttpVersion));

            Console.WriteLine($"Received status code: {httpRequestDriver.HttpResponse.StatusCode}, HTTP Version: {httpRequestDriver.HttpResponse.Version}");
        }

        private int GetCountOfAvailableRates(string currency)
        {
            // if base is different than 'EUR' then both - 
            // selected exchange rate and EUR rate are present in list of exchange rates
            // I'm not sure if that's an error so I left it as it is
            return currency == "EUR" ? CountOfAllAvailableRates : CountOfAllAvailableRates + 1;
        }
    }
}
