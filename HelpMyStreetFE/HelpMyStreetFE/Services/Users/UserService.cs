using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using HelpMyStreetFE.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using HelpMyStreet.Cache;
using System.Threading;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreetFE.Services.Groups;

namespace HelpMyStreetFE.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;
        private readonly IMemDistCache<User> _memDistCache;
        private readonly IGroupService _groupService;

        private const string CACHE_KEY_PREFIX = "user-service-";

        public UserService(IUserRepository userRepository, ILogger<UserService> logger, IMemDistCache<User> memDistCache, IGroupService groupService)
        {
            _userRepository = userRepository;
            _logger = logger;
            _memDistCache = memDistCache;
            _groupService = groupService;
        }

        public async Task<int> CreateUserAsync(string email, string authId, int referringGroupId, string source)
        {
            _logger.LogInformation($"Creating user {email}, {authId}, {referringGroupId}, {source}");
            return await _userRepository.CreateUser(email, authId, referringGroupId, source);
        }

        public async Task<int> UpdateUserAsync(User user, CancellationToken cancellationToken)
        {
            int val = await _userRepository.UpdateUser(user);
            RefreshUserCache(user.ID, cancellationToken);
            return val;
        }

        public async Task<User> GetUserAsync(int id, CancellationToken cancellationToken)
        {
            User user = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return null;
            }, $"{CACHE_KEY_PREFIX}-user-{id}", RefreshBehaviour.DontRefreshData, cancellationToken, NotInCacheBehaviour.DontGetData);

            if (user != null)
            {
                // Found in cache
                return user;
            }

            user = await _userRepository.GetUser(id);

            await RefreshCacheIfRegistrationIsComplete(id, user, cancellationToken);
;
            return user;
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
            DateTime dob,
            CancellationToken cancellationToken)
        {
            await _userRepository.CreateUserStepTwo(new RegistrationStepTwo
            {
                UserID = id,
                PostalCode = HelpMyStreet.Utils.Utils.PostcodeFormatter.FormatPostcode(postCode),
                FirstName = FormatName(firstName),
                LastName = FormatName(lastName),
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
                DisplayName = FormatName(firstName)
            });
        }

        public async Task CreateUserStepThreeAsync(
            int id,
            List<SupportActivities> activities,
            float supportRadius,
            RegistrationFormVariant registrationFormVariant,
            CancellationToken cancellationToken)
        {
            await _userRepository.CreateUserStepThree(new RegistrationStepThree
            {
                UserID = id,
                Activities = activities,
                SupportRadiusMiles = supportRadius,
                SupportVolunteersByPhone = null,
                UnderlyingMedicalCondition = null,
                RegistrationFormVariant = registrationFormVariant,
            });
        }

        public async Task<int> GetVolunteers()
        {
            return await _userRepository.GetDistinctVolunteerUserCount();
        }

        public async Task<GetHelpersByPostcodeResponse> GetHelpersByPostcode(string postcode)
        {
            return await _userRepository.GetHelpersByPostcode(postcode);
        }


        public async Task<VolunteerCoordinatesResponse> GetVolunteerCoordinates(double swLatitude, double swLongitude, double neLatitude, double neLongitude, int minDistanceBetweenInMetres)
        {
            return await _userRepository.GetVolunteerCoordinates(swLatitude, swLongitude, neLatitude, neLongitude, minDistanceBetweenInMetres);
        }

        public async Task<Models.Account.UserDetails> GetUserDetails(User user, CancellationToken cancellationToken)
        {
            var userDetails = new Models.Account.UserDetails(user);
            var locations = await _groupService.GetUserLocations(user.ID);

            userDetails.ShiftsEnabled = locations.Count() > 0;

            return userDetails;
        }

        public string FormatName(string name)
        {
            return string.Join(' ', name.Trim().Split(' ').Where(word => word.Length > 0).Select(word => char.ToUpper(word[0]) + word.Substring(1)));
        }

        public bool GetRegistrationIsComplete(User user)
        {
            if (user?.RegistrationHistory == null)
            {
                return false;
            }
            
            return user.RegistrationHistory.Count > 0 && user.RegistrationHistory.Max(a => a.Key) > 2;
        }

        private void RefreshUserCache(int userId, CancellationToken cancellationToken)
        {
            _memDistCache.RefreshDataAsync(async (cancellationToken) =>
            {
                return await _userRepository.GetUser(userId);
            }, $"{CACHE_KEY_PREFIX}-user-{userId}", cancellationToken);
        }

        public async Task<User> GetUserByAuthId(string authId)
        {
            return await _userRepository.GetUserByAuthId(authId);
        }

        public async Task<bool> AddBiography(int userId, string details)
        {
            _logger.LogInformation($"adding biography for user {userId}");
            var result =  await _userRepository.AddBiography(userId, details);

            var user =  await _userRepository.GetUser(userId);

            await RefreshCacheIfRegistrationIsComplete(userId, user, CancellationToken.None);
            return result;
        }

        private async Task RefreshCacheIfRegistrationIsComplete(int id, User user, CancellationToken cancellationToken)
        {
            if (GetRegistrationIsComplete(user))
            {
                // Don't put users into the cache until registration is complete
                await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return user;
                }, $"{CACHE_KEY_PREFIX}-user-{id}", cancellationToken);
            }
        }
    }
}
