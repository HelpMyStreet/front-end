using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IUserService
    {
        Task CreateUser(string email, string authId);
    }
}