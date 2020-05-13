using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Services.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HelpMyStreetFE.Repositories
{
    public abstract class BaseHttpRepository
    {
        protected readonly HttpClient Client;
        protected readonly ILogger<BaseHttpRepository> Logger;

        protected BaseHttpRepository(HttpClient client,IConfiguration config, ILogger<BaseHttpRepository> logger, string configKey)
        {
            Client = client;
            Logger = logger;
            HttpFunctionServiceOptions options = new HttpFunctionServiceOptions();

            config.GetSection(configKey).Bind(options);

            Client.DefaultRequestHeaders.Add("x-functions-key", options.Key);
            Client.BaseAddress = new Uri(options.Location);
        }

        protected async Task<TResponse> PostAsync<TResponse>(string url, object obj)
        {
            var data = JsonConvert.SerializeObject(obj);
            Logger.LogInformation($"Post request to {url} with {data}");
            var resp = await Client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
            Logger.LogInformation($"Request code: {resp.StatusCode}");

            return await HandleResponseAsync<TResponse>(resp);
        }

        protected async Task<TResponse> PutAsync<TResponse>(string url, object obj)
        {
            var data = JsonConvert.SerializeObject(obj);
            Logger.LogInformation($"Put request to {url} with {data}");
            var resp = await Client.PutAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));
            Logger.LogInformation($"Request code: {resp.StatusCode}");

            return await HandleResponseAsync<TResponse>(resp);
        }

        protected async Task<HttpResponseMessage> GetAsync(string url)
        {
            Logger.LogInformation($"Get request to {url}");
            var resp = await Client.GetAsync(url);
            Logger.LogInformation($"Request code: {resp.StatusCode}");
            return resp;
        }

        protected async Task<TResponse> GetAsync<TResponse>(string url)
        {
            Logger.LogInformation($"Get request to {url}");
            var resp = await Client.GetAsync(url);
            Logger.LogInformation($"Request code: {resp.StatusCode}");
            return await HandleResponseAsync<TResponse>(resp);
        }

        private async Task<TResponse> HandleResponseAsync<TResponse>(HttpResponseMessage resp)
        {
            if (resp.IsSuccessStatusCode)
            {
                var t = await resp.Content.ReadAsStringAsync();
                var respObj = JsonConvert.DeserializeObject<TResponse>(await resp.Content.ReadAsStringAsync());
                return respObj;
            }
            else
            {
                Logger.LogError($"Request failed with code {resp.StatusCode}\n{resp}");
                throw new HttpRequestException($"Request failed with code {resp.StatusCode}");
            }
        }
    }
}
