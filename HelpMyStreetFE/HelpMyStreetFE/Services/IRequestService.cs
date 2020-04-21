using System.Threading.Tasks;
using HelpMyStreetFE.Models.RequestHelp;

namespace HelpMyStreetFE.Services
{
    public interface IRequestService
    {
        Task<Request> LogRequestAsync(string postcode);

		void UpdateRequest(RequestHelpFormModel requestHelpFormModel);
	}
}