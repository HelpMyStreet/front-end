﻿using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Models.Reponses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IAddressService
    {
        Task<GetPostCodeResponse> CheckPostCode(string postCode);
        Task<int> GetPostCodesCovered();
        Task<List<PostCodeDetail>> GetPostcodeDetailsNearUser(User user);
        Task<int> GetStreetChampions();
        Task<int> GetStreetsCovered();
        Task<int> GetStreetsRemaining();
    }
}
