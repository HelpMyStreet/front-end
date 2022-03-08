using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;

namespace HelpMyStreetFE.Repositories
{
    public interface IUserRepository
    {
        Task<UpdateBiographyOutcome> AddBiography(int userId, string details);
        Task<int> CreateUser(string email, string authId, int referringGroupId, string source);
        Task<int> CreateUserStepThree(RegistrationStepThree data);
        Task<int> CreateUserStepTwo(RegistrationStepTwo data);
        Task<int> GetVolunteerCountByPostcode(string postcode);
        Task<User> GetUser(int id);
        Task<User> GetUserByAuthId(string authId);
        Task<int> UpdateUser(User user);
        Task<int> GetDistinctVolunteerUserCount();
        Task<GetHelpersByPostcodeResponse> GetHelpersByPostcode(string postcode);
        Task<VolunteerCoordinatesResponse> GetVolunteerCoordinates(double swLatitude, double swLongitude, double neLatitude, double neLongitude, int minDistanceBetweenInMetres);
    }
}