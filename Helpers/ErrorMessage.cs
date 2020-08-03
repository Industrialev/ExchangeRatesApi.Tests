using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExchangeRatesApi.Tests.Helpers
{
    public class ErrorMessage
    {
        [JsonProperty("error")]
        public string Content;
    }
}
