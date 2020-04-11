using HelpMyStreetFE.Models.Validation;
using HelpMyStreetFE.Models.Yoti;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class ValidationService : BaseHttpService, IValidationService
    {
        private readonly IValidationRepository _validationRepository;
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(ILogger<ValidationService> logger, IConfiguration config, IValidationRepository validationRepository) : base(config, "Services:Validation")
        {
            _logger = logger;
            _validationRepository = validationRepository;
        }

        public async Task<ValidationResponse> ValidateUserAsync(ValidationRequest request, CancellationToken token = default)
        {
            try
            {
                var check = CheckValidationRequest(request);
                if (check != null) return check;

                var response = await _validationRepository.ValidateUser(request);

                return handleResponse(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An unexpected error occurred vaidating user");
                return ValidationResponse.UnexepectedError(e.Message);
            }
        }

        private ValidationResponse handleResponse(HttpResponseMessage responseMessage)
        {
            return responseMessage.StatusCode switch
            {
                HttpStatusCode.OK => ValidationResponse.Success(),
                HttpStatusCode.Unauthorized => ValidationResponse.Unauthorized(),
                _ => ValidationResponse.UnexepectedError()
            };
        }

        private ValidationResponse CheckValidationRequest(ValidationRequest request)
        {
            if (request is null) return ValidationResponse.ValidationFailed("Request object is null");
            if(string.IsNullOrEmpty(request.Token)) return ValidationResponse.ValidationFailed("Token is null or empty");
            if (string.IsNullOrEmpty(request.UserId)) return ValidationResponse.ValidationFailed("UserId is null or empty");

            return null;
        }
    }
}
