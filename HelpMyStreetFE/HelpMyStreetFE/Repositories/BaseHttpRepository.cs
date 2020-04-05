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
        protected readonly HttpClient Client = new HttpClient();
        protected readonly ILogger<BaseHttpRepository> Logger;

        protected BaseHttpRepository(IConfiguration config, ILogger<BaseHttpRepository> logger, string configKey)
        {
            Logger = logger;
            HttpFunctionServiceOptions options = new HttpFunctionServiceOptions();

            config.GetSection(configKey).Bind(options);

            Client.DefaultRequestHeaders.Add("x-functions-key", options.Key);
            Client.BaseAddress = new Uri(options.Location);
        }

        protected async Task<TResponse> PostAsync<TResponse>(string url, object obj)
            where TResponse : BaseResponse, new()
        {
            Logger.LogInformation($"Post request to {url}");
            var resp = await Client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));

            return await HandleResponseAsync<TResponse>(resp);
        }

        protected async Task<TResponse> PutAsync<TResponse>(string url, object obj)
            where TResponse : BaseResponse, new()
        {
            Logger.LogInformation($"Put request to {url}");
            var resp = await Client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));

            return await HandleResponseAsync<TResponse>(resp);
        }

        protected async Task<TResponse> GetAsync<TResponse>(string url)
            where TResponse : BaseResponse, new()
        {
            Logger.LogInformation($"Get request to {url}");
            var resp = await Client.GetAsync(url);

            return await HandleResponseAsync<TResponse>(resp);
        }

        private async Task<TResponse> HandleResponseAsync<TResponse>(HttpResponseMessage resp)
            where TResponse : BaseResponse, new()
        {
            if (resp.IsSuccessStatusCode)
            {
                var respObj = JsonConvert.DeserializeObject<TResponse>(await resp.Content.ReadAsStringAsync());
                respObj.Ok = true;
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
