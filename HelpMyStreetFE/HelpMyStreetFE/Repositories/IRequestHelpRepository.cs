using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
	public interface IRequestHelpRepository
	{
		Task<LogRequestResponse> LogRequest(HelpMyStreet.Contracts.RequestService.Request.PostNewRequestForHelpRequest request);

	}
}


