using HelpMyStreetFE.Models.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class ValidationService : BaseHttpService, IValidationService
    {
        private readonly ILogger<ValidationService> _logger;
        public ValidationService(ILogger<ValidationService> logger, IConfiguration config) : base(config, "Services:Validation")
        {
            _logger = logger;
        }

        public async Task<ValidationResponse> ValidateUser(ValidationRequest request, CancellationToken token = default)
        {
            var check = CheckValidationRequest(request);
            if (check != null) return check;

            var url = $"/api/getValidation/{request.UserId}/{request.Token}";
            var response = await Client.PostAsync(url, null);

            throw new NotImplementedException();
        }

        private ValidationResponse CheckValidationRequest(ValidationRequest request)
        {
            if (request is null) return ValidationResponse.Failed("Request object is null");
            if(string.IsNullOrEmpty(request.Token)) return ValidationResponse.Failed("Token is null or empty");
            if (string.IsNullOrEmpty(request.UserId)) return ValidationResponse.Failed("UserId is null or empty");

            return null;
        }
    }
}
