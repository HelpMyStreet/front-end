using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public class UserRepository : BaseHttpRepository, IUserRepository
    {
        public UserRepository(IConfiguration config, ILogger<UserRepository> logger) : base(config, logger, "Services:User")
        { }

        public async Task<User> GetUserByAuthId(string authId)
        {
            var resp = await GetAsync<GetUserResponse>($"/api/getuserbyfirebaseuserid?firebaseuid{authId}");

            return resp.User;
        }

        public async Task<int> CreateUser(string email, string authId)
        {
            var response = await PostAsync<ModifyUserResponse>("/api/postcreateuser", new
            {
                EmailAddress = email,
                FirebaseUID = authId
            });

            return response.Id;
        }

        public async Task<int> UpdateUser(User user)
        {
            var response = await PutAsync<ModifyUserResponse>("/api/putmodifyuser", new { user });

            return response.Id;
        }
    }
}
