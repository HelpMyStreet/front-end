using HelpMyStreet.Contracts.RequestService.Request;

using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
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
				RequestId = response.Content.RequestID,
				Fulfillable = response.Content.Fulfillable
			};
		}

		public async Task<string> UpdateRequest(RequestHelpFormModel requestHelpFormModel)
		{

			var response = await PostAsync<string>($"/api/updaterequest",

				new UpdateRequestRequest
				{
					RequestID = requestHelpFormModel.RequestId,
					FurtherDetails = requestHelpFormModel.Message,
					HealthOrWellbeingConcern = requestHelpFormModel.HealthConcern,
					OnBehalfOfAnother = !requestHelpFormModel.HelpForMe,
					RequestorEmailAddress = requestHelpFormModel.Email,
					RequestorFirstName = requestHelpFormModel.FirstName,
					RequestorLastName = requestHelpFormModel.LastName,
					RequestorPhoneNumber = requestHelpFormModel.PhoneNumber,
					SupportActivitiesRequired = new SupportActivityRequest
					{
						SupportActivities = requestHelpFormModel.HelpNeeded
					}					
				});

			return response;
		}
	}
}
