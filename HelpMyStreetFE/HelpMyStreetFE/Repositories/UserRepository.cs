using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Models.Reponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public class UserRepository : BaseHttpRepository, IUserRepository
    {
        public UserRepository(HttpClient client, IConfiguration config, ILogger<UserRepository> logger) : base(client, config, logger, "Services:User")
        { }

        public async Task<User> GetUserByAuthId(string authId)
        {
            var resp = await GetAsync<GetUserResponse>($"/api/getuserbyfirebaseuserid?firebaseuid={authId}");

            return resp.User;
        }

        public async Task<User> GetUser(int id)
        {
            var resp = await GetAsync<GetUserResponse>($"/api/getuserbyid?id={id}");

            return resp.User;
        }

        public async Task<int> CreateUser(string email, string authId)
        {
            var response = await PostAsync<ModifyUserResponse>("/api/postcreateuser", new
            {
                RegistrationStepOne = new RegistrationStepOne
                {
                    EmailAddress = email,
                    FirebaseUID = authId,
                    DateCreated = DateTime.Now
                }
            });

            return response.Id;
        }

        public async Task<int> CreateUserStepTwo(RegistrationStepTwo data)
        {
            var response = await PutAsync<ModifyUserResponse>("/api/PutModifyRegistrationPageTwo", new
            {
                RegistrationStepTwo = data
            });

            return response.Id;
        }

        public async Task<int> CreateUserStepThree(RegistrationStepThree data)
        {
            var response = await PutAsync<ModifyUserResponse>("/api/PutModifyRegistrationPageThree", new
            {
                RegistrationStepThree = data
            });

            return response.Id;
        }

        public async Task<int> CreateUserStepFour(RegistrationStepFour data)
        {
            var response = await PutAsync<ModifyUserResponse>("/api/PutModifyRegistrationPageFour", new
            {
                RegistrationStepFour = data
            });

            return response.Id;
        }

        public async Task<int> CreateUserStepFive(RegistrationStepFive data)
        {
            var response = await PutAsync<ModifyUserResponse>("/api/PutModifyRegistrationPageFive", new
            {
                RegistrationStepFive = data
            });

            return response.Id;
        }

        public async Task<int> UpdateUser(User user)
        {
            var response = await PutAsync<ModifyUserResponse>("/api/putmodifyuser", new { user });

            return response.Id;
        }

        public async Task<int> GetChampionCountByPostcode(string postcode)
        {
            var response = await GetAsync<GetCountResponse>($"/api/getchampioncountbypostcode?postcode={postcode}");

            return response.Count;
        }

        public async Task<int> GetDistinctChampionUserCount()
        {
            var response = await GetAsync<GetCountResponse>($"/api/GetDistinctChampionUserCount");

            return response.Count;
        }

        public async Task<int> GetChampionPostcodesCoveredCount()
        {
            var response = await GetAsync<GetCountResponse>($"/api/GetChampionPostcodesCoveredCount");

            return response.Count;
        }

        public async Task<int> GetDistinctVolunteerUserCount()
        {
            var response = await GetAsync<GetCountResponse>($"/api/GetDistinctVolunteerUserCount");

            return response.Count;
        }

        public async Task<int> GetVolunteerCountByPostcode(string postcode)
        {
            var response = await GetAsync<GetCountResponse>($"/api/GetVolunteerCountByPostcode?postcode={postcode}");
            return response.Count;
        }

        public async Task<GetHelperResponse> GetHelpersByPostcode(string postcode)
        {
            var response = await GetAsync<GetHelperResponse>($"/api/GetHelpersByPostcode?postCode={postcode}");
            return response;
        }

        public async Task<GetHelperResponse> GetChampionsByPostcode(string postcode)
        {
            var response = await GetAsync<GetHelperResponse>($"/api/GetChampionsByPostcode?postCode={postcode}");
            return response;
        }

        public async Task<VolunteerCoordinatesResponse> GetVolunteerCoordinates(double swLatitude, double swLongitude, double neLatitude, double neLongitude, int minDistanceBetweenInMetres)
        {
            VolunteerCoordinatesResponse response = await GetAsync<VolunteerCoordinatesResponse>($"/api/GetVolunteerCoordinates?SWLatitude={swLatitude}&SWLongitude={swLongitude}&NELatitude={neLatitude}&NELongitude={neLongitude}&MinDistanceBetweenInMetres={minDistanceBetweenInMetres}&VolunteerType=3&IsVerifiedType=3");
            return response;
        }
    }
}
