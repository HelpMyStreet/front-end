using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;

namespace HelpMyStreetFE.Repositories
{
    public interface IUserRepository
    {
        Task<int> CreateUser(string email, string authId);
        Task<User> GetUserByAuthId(string authId);
        Task<int> UpdateUser(User user);
    }
}