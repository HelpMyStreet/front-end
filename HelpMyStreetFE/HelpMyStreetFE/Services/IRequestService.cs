using System.Threading.Tasks;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;

namespace HelpMyStreetFE.Services
{
    public interface IRequestService
    {
        Task<LogRequestResponse> LogRequestAsync(string postcode);

        Task<UpdateRequestResponse> UpdateRequest(RequestHelpFormModel requestHelpFormModel);
	}
}