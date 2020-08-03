using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace ExchangeRatesApi.Tests.Drivers
{
    public class HttpRequestDriver
    {
        private static HttpClient client;
        private UriBuilder uriBuilder;

        public HttpResponseMessage HttpResponse;

        public NameValueCollection Query { get; private set; }

        public HttpRequestDriver(string baseUri)
        {
            client = new HttpClient();
            client.BaseAddress = new Uri(baseUri);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            uriBuilder = new UriBuilder(baseUri);
            Query = HttpUtility.ParseQueryString(uriBuilder.Query);
        }

        public void AddElementToQuery(string key, string value)
        {
            Query[key] = value;
        }

        public void SendGetRequest()
        {
            uriBuilder.Query = Query.ToString();
            HttpResponse = client.GetAsync(uriBuilder.ToString()).Result;
        }

        public T GetDeserializedResponse<T>()
        {
            var contentStream = HttpResponse.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(contentStream);
        }

        public void Dispose()
        {
            client.Dispose();
        }
    }
}
