using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Request;
namespace HelpMyStreetFE.Repositories
{
	public interface IRequestHelpRepository
	{
		Task<LogRequestResponse> LogRequest(PostNewRequestForHelpRequest request);

	}
}


