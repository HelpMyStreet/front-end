using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
	public interface IRequestHelpRepository
	{
		Task<LogRequestResponse> LogRequest(string postcode);

		Task<UpdateRequestResponse> UpdateRequest(RequestHelpFormModel requestHelpFormModel);
	}
}


