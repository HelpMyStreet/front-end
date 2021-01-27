﻿using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Models.Reponses;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.AddressService.Request;
using HelpMyStreet.Contracts.AddressService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using System.Threading;
using HelpMyStreetFE.Models.Account;

namespace HelpMyStreetFE.Services
{
    public interface IAddressService
    {
        Task<GetPostCodeResponse> CheckPostCode(string postCode);
        Task<int> GetTotalStreets();
        Task<GetPostCodeCoverageResponse> GetPostcodeCoverage(string postcode);
        Task<GetPostcodesResponse> GetFriendlyNames(List<string> postcodes);
        Task<ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>> GetPostcodeCoordinates(GetPostcodeCoordinatesRequest getPostcodeCoordinatesRequest);
        Task<ResponseWrapper<GetPostcodeCoordinatesResponse, AddressServiceErrorCode>> GetPostcodeCoordinate(string postcode);
        Task<List<LocationDistance>> GetLocationsForUser(User user, CancellationToken cancellationToken);
        Task<IEnumerable<LocationWithDistance>> GetLocationDetailsForUser(User user, CancellationToken cancellationToken);
        Task<LocationDetails> GetLocationDetails(Location location, CancellationToken cancellationToken);
        Task<List<LocationDetails>> GetLocationDetails(IEnumerable<Location> locations, CancellationToken cancellationToken);
    }
}
