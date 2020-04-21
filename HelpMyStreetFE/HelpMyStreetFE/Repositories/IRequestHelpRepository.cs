using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
	public interface IRequestHelpRepository
	{
		Task<Request> LogRequest(string postcode);
	}
}


