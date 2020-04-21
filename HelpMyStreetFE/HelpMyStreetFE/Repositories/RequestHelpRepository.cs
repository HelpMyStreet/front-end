using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public class RequestHelpRepository : BaseHttpRepository, IRequestHelpRepository
    {
        public RequestHelpRepository(HttpClient client, IConfiguration config, ILogger<RequestHelpRepository> logger) : base(client, config, logger, "Services:Request")
        { }

        public async Task<Request> LogRequest(string postcode)
        {

            //    return new Request
            //    {
            //        RequestId = 1,
            //        Fulfillable = true
            //    };

            var response = await PostAsync<LogRequestResponse>($"/api/logrequest", new
            {
                postcode
            });

            return new Request
            {
                RequestId = response.RequestID,
                Fulfillable = response.Fulfillable
            };
        }
    }
}
