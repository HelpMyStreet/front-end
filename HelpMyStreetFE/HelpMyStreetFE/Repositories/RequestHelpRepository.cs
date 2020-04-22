using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.Requests;
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

		public async Task<string> UpdateRequest(RequestHelpFormModel requestHelpFormModel)
		{
			var response = await PostAsync<string>($"/api/updaterequest",
				new UpdateRequestRequest
				{
					requestId = requestHelpFormModel.RequestId,
					furtherDetails = requestHelpFormModel.Message,
					healthOrWellbeingConcern = requestHelpFormModel.HealthConcern,
					onBehalfOfAnother = !requestHelpFormModel.HelpForMe,
					requestorEmailAddress = requestHelpFormModel.Email,
					requestorFirstName = requestHelpFormModel.FirstName,
					requestorLastName = requestHelpFormModel.LastName,
					requestorPhoneNumber = requestHelpFormModel.PhoneNumber,
					supportActivitiesRequired = requestHelpFormModel.HelpNeeded.Select(itm => itm.ToString()).ToArray()
				});

			return response;
		}
	}
}
