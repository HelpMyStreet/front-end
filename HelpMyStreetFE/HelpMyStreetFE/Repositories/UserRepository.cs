using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Contracts.UserService.Request;
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
            var response = await GetAsync<ResponseWrapper<GetUserByFirebaseUIDResponse,UserServiceErrorCode>>($"/api/getuserbyfirebaseuserid?firebaseuid={authId}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.User;
            }
            return null;
        }

        public async Task<User> GetUser(int id)
        {
            var response = await GetAsync<ResponseWrapper<GetUserByIDResponse, UserServiceErrorCode>>($"/api/getuserbyid?id={id}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.User;
            }
            return null;
        }

        public async Task<int> CreateUser(string email, string authId, int referringGroupId, string source)
        {
            var request = new PostCreateUserRequest()
            {
                RegistrationStepOne = new RegistrationStepOne
                {
                    EmailAddress = email,
                    FirebaseUID = authId,
                    ReferringGroupId = referringGroupId,
                    Source = source,
                    DateCreated = DateTime.Now
                }
            };

            var response = await PostAsync<ResponseWrapper<PostCreateUserResponse,UserServiceErrorCode>>("/api/postcreateuser", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.ID;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }

        }

        public async Task<int> CreateUserStepTwo(RegistrationStepTwo data)
        {
            var request = new PutModifyRegistrationPageTwoRequest()
            {
                RegistrationStepTwo = data
            };

            var response = await PutAsync<ResponseWrapper<PutModifyRegistrationPageTwoResponse, UserServiceErrorCode>>("/api/PutModifyRegistrationPageTwo", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.ID;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<int> CreateUserStepThree(RegistrationStepThree data)
        {
            var request = new PutModifyRegistrationPageThreeRequest()
            {
                RegistrationStepThree = data
            };

            var response = await PutAsync<ResponseWrapper<PutModifyRegistrationPageThreeResponse, UserServiceErrorCode>>("/api/PutModifyRegistrationPageThree", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.ID;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<int> CreateUserStepFour(RegistrationStepFour data)
        {
            var request = new PutModifyRegistrationPageFourRequest()
            {
                RegistrationStepFour = data
            };

            var response = await PutAsync<ResponseWrapper<PutModifyRegistrationPageFourResponse, UserServiceErrorCode>>("/api/PutModifyRegistrationPageFour", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.ID;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<int> CreateUserStepFive(RegistrationStepFive data)
        {
            var request = new PutModifyRegistrationPageFiveRequest()
            {
                RegistrationStepFive = data
            };

            var response = await PutAsync<ResponseWrapper<PutModifyRegistrationPageFiveResponse, UserServiceErrorCode>>("/api/PutModifyRegistrationPageFive", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.ID;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<int> UpdateUser(User user)
        {
            var request = new PutModifyUserRequest()
            {
                User = user
            };

            var response = await PutAsync<ResponseWrapper<PutModifyUserResponse, UserServiceErrorCode>>("/api/putmodifyuser", request);

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.UserID;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<int> GetChampionCountByPostcode(string postcode)
        {
            var response = await GetAsync<ResponseWrapper<GetChampionCountByPostcodeResponse, UserServiceErrorCode>>($"/api/getchampioncountbypostcode?postcode={postcode}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Count;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<int> GetDistinctChampionUserCount()
        {
            var response = await GetAsync<ResponseWrapper<GetDistinctChampionUserCountResponse, UserServiceErrorCode>>($"/api/GetDistinctChampionUserCount");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Count;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<int> GetChampionPostcodesCoveredCount()
        {
            var response = await GetAsync<ResponseWrapper<GetChampionPostcodesCoveredCountResponse, UserServiceErrorCode>>($"/api/GetChampionPostcodesCoveredCount");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Count;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<int> GetDistinctVolunteerUserCount()
        {
            var response = await GetAsync<ResponseWrapper<GetDistinctVolunteerUserCountResponse, UserServiceErrorCode>>($"/api/GetDistinctVolunteerUserCount");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Count;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<int> GetVolunteerCountByPostcode(string postcode)
        {
            var response = await GetAsync<ResponseWrapper<GetVolunteerCountByPostcodeResponse, UserServiceErrorCode>>($"/api/GetVolunteerCountByPostcode?postcode={postcode}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content.Count;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<GetHelpersByPostcodeResponse> GetHelpersByPostcode(string postcode)
        {
            var response = await GetAsync<ResponseWrapper<GetHelpersByPostcodeResponse, UserServiceErrorCode>>($"/api/GetHelpersByPostcode?postCode={postcode}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<GetChampionsByPostcodeResponse> GetChampionsByPostcode(string postcode)
        {
            var response = await GetAsync<ResponseWrapper<GetChampionsByPostcodeResponse, UserServiceErrorCode>>($"/api/GetChampionsByPostcode?postCode={postcode}");

            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }

        public async Task<VolunteerCoordinatesResponse> GetVolunteerCoordinates(double swLatitude, double swLongitude, double neLatitude, double neLongitude, int minDistanceBetweenInMetres)
        {
            var response = await GetAsync<ResponseWrapper<VolunteerCoordinatesResponse, UserServiceErrorCode>>($"/api/GetVolunteerCoordinates?SWLatitude={swLatitude}&SWLongitude={swLongitude}&NELatitude={neLatitude}&NELongitude={neLongitude}&MinDistanceBetweenInMetres={minDistanceBetweenInMetres}&VolunteerType=3&IsVerifiedType=3");
            if (response.HasContent && response.IsSuccessful)
            {
                return response.Content;
            }
            else
            {
                throw new Exception(response.Errors.ToString());
            }
        }
    }
}
