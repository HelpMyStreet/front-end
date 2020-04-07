using HelpMyStreetFE.Models.Validation;
using HelpMyStreetFE.Models.Yoti;
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
        private readonly ILogger<ValidationService> _logger;
        public ValidationService(ILogger<ValidationService> logger, IConfiguration config) : base(config, "Services:Validation")
        {
            _logger = logger;
        }

        public async Task<ValidationResponse> ValidateUserAsync(ValidationRequest request, CancellationToken token = default)
        {
            try
            {
                var check = CheckValidationRequest(request);
                if (check != null) return check;

                var url = $"/api/getValidation/{request.UserId}/{request.Token}";
                var response = await Client.PostAsync(url, null);

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
