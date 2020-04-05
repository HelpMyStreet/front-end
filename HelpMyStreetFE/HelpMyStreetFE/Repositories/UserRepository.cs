using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Registration;
using HelpMyStreetFE.Models.Reponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public class UserRepository : BaseHttpRepository, IUserRepository
    {
        public UserRepository(IConfiguration config, ILogger<UserRepository> logger) : base(config, logger, "Services:User")
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
    }
}
