using HelpMyStreet.Contracts.RequestService.Request;

using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
	public class RequestHelpRepository : BaseHttpRepository, IRequestHelpRepository
	{
		public RequestHelpRepository(HttpClient client, IConfiguration config, ILogger<RequestHelpRepository> logger) : base(client, config, logger, "Services:Request")
		{ }

		public async Task<LogRequestResponse> LogRequest(string postcode)
		{
			postcode = HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(postcode);
			var response = await PostAsync<LogRequestResponse>($"/api/logrequest", new
			{
				postcode
			});

			return response;
    }
		
	}
}
