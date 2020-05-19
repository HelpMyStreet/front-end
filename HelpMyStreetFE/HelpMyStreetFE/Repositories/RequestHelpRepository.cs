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

	

		public async Task<LogRequestResponse> LogRequest(PostNewRequestForHelpRequest request)
		{
			var response = await PostAsync<LogRequestResponse>($"/api/PostNewRequestForHelp", request);
			return response;
		}

	}
}
