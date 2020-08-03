using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeRatesApi.Tests.Helpers
{
    public class ExchangeRates
    {
        [JsonProperty("rates")]
        public Dictionary<string, double> Rates;

        [JsonProperty("base")]
        public string Base { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }
    }
}
