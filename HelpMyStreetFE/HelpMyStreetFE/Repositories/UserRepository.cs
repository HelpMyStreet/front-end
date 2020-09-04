using HelpMyStreet.Contracts.UserService.Response;
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
            var resp = await GetAsync<GetUserByFirebaseUIDResponse>($"/api/getuserbyfirebaseuserid?firebaseuid={authId}");

            return resp.User;
        }

        public async Task<User> GetUser(int id)
        {
            try
            {
                var resp = await GetAsync<GetUserByIDResponse>($"/api/getuserbyid?id={id}");
                return resp.User;
            }
            catch
            {
                return null;
            }
        }

        public async Task<int> CreateUser(string email, string authId, int referringGroupId, string source)
        {
            var response = await PostAsync<PostCreateUserResponse>("/api/postcreateuser", new
            {
                RegistrationStepOne = new RegistrationStepOne
                {
                    EmailAddress = email,
                    FirebaseUID = authId,
                    ReferringGroupId = referringGroupId,
                    Source = source,
                    DateCreated = DateTime.Now
                }
            });

            return response.ID;
        }

        public async Task<int> CreateUserStepTwo(RegistrationStepTwo data)
        {
            var response = await PutAsync<PutModifyRegistrationPageTwoResponse>("/api/PutModifyRegistrationPageTwo", new
            {
                RegistrationStepTwo = data
            });

            return response.ID;
        }

        public async Task<int> CreateUserStepThree(RegistrationStepThree data)
        {
            var response = await PutAsync<PutModifyRegistrationPageThreeResponse>("/api/PutModifyRegistrationPageThree", new
            {
                RegistrationStepThree = data
            });

            return response.ID;
        }

        public async Task<int> CreateUserStepFour(RegistrationStepFour data)
        {
            var response = await PutAsync<PutModifyRegistrationPageFourResponse>("/api/PutModifyRegistrationPageFour", new
            {
                RegistrationStepFour = data
            });

            return response.ID;
        }

        public async Task<int> CreateUserStepFive(RegistrationStepFive data)
        {
            var response = await PutAsync<PutModifyRegistrationPageFiveResponse>("/api/PutModifyRegistrationPageFive", new
            {
                RegistrationStepFive = data
            });

            return response.ID;
        }

        public async Task<int> UpdateUser(User user)
        {
            var response = await PutAsync<PutModifyUserResponse>("/api/putmodifyuser", new { user });

            return response.UserID;
        }

        public async Task<int> GetChampionCountByPostcode(string postcode)
        {
            var response = await GetAsync<GetChampionCountByPostcodeResponse>($"/api/getchampioncountbypostcode?postcode={postcode}");

            return response.Count;
        }

        public async Task<int> GetDistinctChampionUserCount()
        {
            var response = await GetAsync<GetDistinctChampionUserCountResponse>($"/api/GetDistinctChampionUserCount");

            return response.Count;
        }

        public async Task<int> GetChampionPostcodesCoveredCount()
        {
            var response = await GetAsync<GetChampionPostcodesCoveredCountResponse>($"/api/GetChampionPostcodesCoveredCount");

            return response.Count;
        }

        public async Task<int> GetDistinctVolunteerUserCount()
        {
            var response = await GetAsync<GetDistinctVolunteerUserCountResponse>($"/api/GetDistinctVolunteerUserCount");

            return response.Count;
        }

        public async Task<int> GetVolunteerCountByPostcode(string postcode)
        {
            var response = await GetAsync<GetVolunteerCountByPostcodeResponse>($"/api/GetVolunteerCountByPostcode?postcode={postcode}");
            return response.Count;
        }

        public async Task<GetHelpersByPostcodeResponse> GetHelpersByPostcode(string postcode)
        {
            var response = await GetAsync<GetHelpersByPostcodeResponse>($"/api/GetHelpersByPostcode?postCode={postcode}");
            return response;
        }

        public async Task<GetChampionsByPostcodeResponse> GetChampionsByPostcode(string postcode)
        {
            var response = await GetAsync<GetChampionsByPostcodeResponse>($"/api/GetChampionsByPostcode?postCode={postcode}");
            return response;
        }

        public async Task<VolunteerCoordinatesResponse> GetVolunteerCoordinates(double swLatitude, double swLongitude, double neLatitude, double neLongitude, int minDistanceBetweenInMetres)
        {
            VolunteerCoordinatesResponse response = await GetAsync<VolunteerCoordinatesResponse>($"/api/GetVolunteerCoordinates?SWLatitude={swLatitude}&SWLongitude={swLongitude}&NELatitude={neLatitude}&NELongitude={neLongitude}&MinDistanceBetweenInMetres={minDistanceBetweenInMetres}&VolunteerType=3&IsVerifiedType=3");
            return response;
        }
    }
}
