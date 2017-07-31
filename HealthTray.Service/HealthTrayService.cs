using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using HealthTray.Service.Model;

namespace HealthTray.Service
{
    /// <summary>
    /// Retrieves <see cref="Check"/> objects from the healthchecks.io REST API.
    /// </summary>
    public class HealthTrayService : BaseService, IHealthTrayService
    {
        internal HealthTrayService(HttpClient httpClient) : this(httpClient, "http://localhost.fiddler", null) { }

        public HealthTrayService(string baseUrl, string apiKey) : this(new HttpClient(), baseUrl, apiKey) { }

        public HealthTrayService(HttpClient httpClient, string baseUrl, string apiKey)
        {
            HttpClient = httpClient;
            if (HttpClient.BaseAddress == null) HttpClient.BaseAddress = new Uri(baseUrl);
            HttpClient.DefaultRequestHeaders.Add("X-Api-Key", apiKey);
        }

        public async Task<IList<Check>> GetChecks()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "checks");
            string responseContent = await GetResponseContentAsync(request);
            var response = JsonConvert.DeserializeObject<JObject>(responseContent);
            return response["checks"]?.ToObject<IList<Check>>();
        }
    }
}
