using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Models.Requests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
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
					supportActivitiesRequired = new SupportActivityRequest
					{
						supportActivities = requestHelpFormModel.HelpNeeded.Select(itm => itm.ToString()).ToArray()
					}					
				});

			return response;
		}
	}
}
