using HelpMyStreetFE.Models.Validation;
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

namespace HelpMyStreetFE.Services.Users
{
    public class VerificationService : BaseHttpService, IVerificationService
    {
        private readonly IVerificationRepository _validationRepository;
        private readonly ILogger<VerificationService> _logger;

        public VerificationService(
            ILogger<VerificationService> logger, 
            IConfiguration config, 
            IVerificationRepository validationRepository,
            HttpClient client) : base(client,config, "Services:Validation")
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
                _ => throw new Exception($"Unexpected response from Verification Service: {responseMessage.Content}")
            };
        }

        private ValidationResponse CheckValidationRequest(ValidationRequest request)
        {
            if (request is null) return ValidationResponse.ValidationFailed("Request object is null");
            if(string.IsNullOrEmpty(request.Token)) return ValidationResponse.ValidationFailed("Token is null or empty");
            if (request.UserId == 0) return ValidationResponse.ValidationFailed("UserId is zero");

            return null;
        }
    }
}
