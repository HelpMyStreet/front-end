using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using HelpMyStreet.Utils.CoordinatedResetCache;

namespace HelpMyStreetFE.Services
{
    public class GoogleService : IGoogleService
    {
        private readonly ILogger<GoogleService> _logger;
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ICoordinatedResetCache _coordinatedResetCache;

        public GoogleService(
            ILogger<GoogleService> logger,
            IConfiguration configuration,
            HttpClient client,
            ICoordinatedResetCache coordinatedResetCache)
        {
            _client = client;
            _configuration = configuration;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _coordinatedResetCache = coordinatedResetCache;
        }


        public async Task<string> GetCachedGoogleMapScript()
        {
            string key = $"{nameof(GoogleService)}_GoogleMapScript";
            string script = await _coordinatedResetCache.GetCachedDataAsync(async () => await GetGoogleMapScript(), key,  CoordinatedResetCacheTime.OnHour);
            return script;
        }

        private async Task<string> GetGoogleMapScript()
        {
            string key = _configuration.GetValue<string>("GoogleMap:Key");
            string scriptSource = _configuration.GetValue<string>("GoogleMap:ScriptSource");
            string scriptSouceWithKey = $"{scriptSource}?key={key}&callback=initGoogleMap";

            HttpResponseMessage response = await _client.GetAsync(scriptSouceWithKey);
            string str = await response.Content.ReadAsStringAsync();
            return str;
        }

    }
}
