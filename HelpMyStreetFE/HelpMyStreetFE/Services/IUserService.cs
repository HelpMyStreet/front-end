using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account;
using HelpMyStreetFE.Models.Reponses;

namespace HelpMyStreetFE.Services
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(string email, string authId, int referringGroupId, string source);
        Task<int> UpdateUserAsync(User user, CancellationToken cancellationToken);
        Task<User> GetUserAsync(int id, CancellationToken cancellationToken);
        Task CreateUserStepTwoAsync(int id, string postCode, string firstName, string lastName, string addressLine1, string addressLine2, string addressLine3, string locality, string mobile, string otherPhone, System.DateTime dob, CancellationToken cancellationToken);
        Task CreateUserStepThreeAsync(int id, System.Collections.Generic.List<HelpMyStreet.Utils.Enums.SupportActivities> activities, float supportRadius, CancellationToken cancellationToken);
        Task CreateUserStepFourAsync(int id, bool roleUnderstood, System.Collections.Generic.List<string> postcodes, CancellationToken cancellationToken);
        Task CreateUserStepFiveAsync(int id, bool verified, CancellationToken cancellationToken);
        Task<int> GetStreetChampions();
        Task<int> GetStreetsCovered();
        Task<int> GetVolunteers();
        Task<GetHelperResponse> GetHelpersByPostcode(string postcode);
        Task<GetHelperResponse> GetChampionsByPostcode(string postcode);
        Task<VolunteerCoordinatesResponse> GetVolunteerCoordinates(double swLatitude, double swLongitude, double neLatitude, double neLongitude, int minDistanceBetweenInMetres);
        UserDetails GetUserDetails(User user);
        bool GetRegistrationIsComplete(User user);
    }
}