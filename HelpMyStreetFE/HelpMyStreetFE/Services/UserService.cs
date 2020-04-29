using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<int> CreateUserAsync(string email, string authId)
        {
            _logger.LogInformation($"Creating user {email}, {authId}");
            return await _userRepository.CreateUser(email, authId);
        }

        public async Task<int> UpdateUserAsync(User user)
        {
            return await _userRepository.UpdateUser(user);
        }

        public async Task<User> GetUserAsync(int id)
        {
            return await _userRepository.GetUser(id);
        }

        public async Task CreateUserStepTwoAsync(
            int id,
            string postCode,
            string firstName,
            string lastName,
            string addressLine1,
            string addressLine2,
            string addressLine3,
            string locality,
            string mobile,
            string otherPhone,
            DateTime dob)
        {
            await _userRepository.CreateUserStepTwo(new RegistrationStepTwo
            {
                UserID = id,
                PostalCode = HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(postCode),
                FirstName = firstName,
                LastName = lastName,
                Address = new Address
                {
                    AddressLine1 = addressLine1,
                    AddressLine2 = addressLine2 ?? "",
                    AddressLine3 = addressLine3 ?? "",
                    Postcode = HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(postCode),
                    Locality = locality
                },
                MobilePhone = mobile,
                OtherPhone = otherPhone ?? "",
                DateOfBirth = dob,
                DisplayName = $"{firstName}"
            });
        }

        public async Task CreateUserStepThreeAsync(
            int id,
            List<SupportActivities> activities,
            float supportRadius,
            bool supportContact,
            bool medical)
        {
            await _userRepository.CreateUserStepThree(new RegistrationStepThree
            {
                UserID = id,
                Activities = activities,
                SupportRadiusMiles = supportRadius,
                SupportVolunteersByPhone = supportContact,
                UnderlyingMedicalCondition = medical
            });
        }

        public async Task CreateUserStepFourAsync(
            int id,
            bool roleUnderstood,
            List<string> postcodes)
        {
            await _userRepository.CreateUserStepFour(new RegistrationStepFour
            {
                UserID = id,
                StreetChampionRoleUnderstood = roleUnderstood,
                ChampionPostcodes = postcodes
            });
        }

        public async Task CreateUserStepFiveAsync(
         int id,
         bool verified)
        {
            await _userRepository.CreateUserStepFive(new RegistrationStepFive
            {
                UserID = id,
                IsVerified = verified
            });
        }

        public async Task<int> GetStreetChampions()
        {
            return await _userRepository.GetDistinctChampionUserCount();
        }

        public async Task<int> GetStreetsCovered()
        {
            return await _userRepository.GetChampionPostcodesCoveredCount();
        }

        public async Task<int> GetVolunteers()
        {
            return await _userRepository.GetDistinctVolunteerUserCount();
        }

        public async Task<GetHelperResponse> GetHelpersByPostcode(string postcode)
        {
            return await _userRepository.GetHelpersByPostcode(postcode);
        }

        public async Task<GetHelperResponse> GetChampionsByPostcode(string postcode)
        {
            return await _userRepository.GetChampionsByPostcode(postcode);
        }
    }

}
