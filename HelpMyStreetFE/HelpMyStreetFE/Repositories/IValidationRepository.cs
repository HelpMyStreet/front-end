using HelpMyStreetFE.Models.Validation;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public interface IValidationRepository
    {
        Task<HttpResponseMessage> ValidateUser(ValidationRequest request);
    }
}