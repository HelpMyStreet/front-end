using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class UserService : IUserService
    {
        public async Task CreateUser(string email, string authId)
        {
            await Task.CompletedTask;
        }
    }
}
