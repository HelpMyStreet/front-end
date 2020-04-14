using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Services.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;

namespace HelpMyStreetFE.Services
{
    public abstract class BaseHttpService
    {
        protected readonly HttpClient Client;

        protected BaseHttpService(HttpClient client, IConfiguration config, string configKey)
        {
            Client = client;
            HttpFunctionServiceOptions options = new HttpFunctionServiceOptions();

            config.GetSection(configKey).Bind(options);

            Client.DefaultRequestHeaders.Add("x-functions-key", options.Key);
            Client.BaseAddress = new Uri(options.Location);
        }

        protected async Task<TResponse> MakeRequest<TResponse>(string url, object obj)
            where TResponse : BaseResponse, new()
        {
            var resp = await Client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));

            return await HandleResponse<TResponse>(resp);
        }

        protected async Task<TResponse> Put<TResponse>(string url, object obj)
            where TResponse : BaseResponse, new()
        {
            var resp = await Client.PutAsync(url, new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json"));

            return await HandleResponse<TResponse>(resp);
        }

        protected async Task<TResponse> Get<TResponse>(string url)
            where TResponse : BaseResponse, new()
        {
            var resp = await Client.GetAsync(url);

            return await HandleResponse<TResponse>(resp);
        }

        private async Task<TResponse> HandleResponse<TResponse>(HttpResponseMessage resp)
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
                Console.WriteLine("Fail");
                Console.WriteLine(resp);
                return new TResponse
                {
                    StatusCode = resp.StatusCode,
                    Ok = false,
                    Error = "Unknown error"
                };
            }
        }
    }
}
