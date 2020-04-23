using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class RequestService : IRequestService
    {
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly ILogger<RequestService> _logger;

        public RequestService(IRequestHelpRepository requestHelpRepository, ILogger<RequestService> logger)
        {
            _requestHelpRepository = requestHelpRepository;
            _logger = logger;
        }

        public async Task<LogRequestResponse> LogRequestAsync(string postcode)
        {
            _logger.LogInformation($"Logging Request for postcode {postcode}");
            return await _requestHelpRepository.LogRequest(postcode);

        }

        public async Task<UpdateRequestResponse> UpdateRequest(RequestHelpFormModel requestHelpFormModel)
        {
            _logger.LogInformation($"Updating Request with Id {requestHelpFormModel.RequestId}");
            var result =  await _requestHelpRepository.UpdateRequest(requestHelpFormModel);

            return result;
        }
    }
}
