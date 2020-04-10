using HelpMyStreetFE.Models.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public class ValidationRepository : BaseHttpRepository, IValidationRepository
    {
        public ValidationRepository(IConfiguration config, ILogger<ValidationRepository> logger) : base(config, logger, "Services:Validation")
        {
        }

        public async Task<HttpResponseMessage> ValidateUser(ValidationRequest request)
        {
            return await GetAsync<HttpResponseMessage>($"/api/getValidation/{request.UserId}/{request.Token}");
        }
    }
}
