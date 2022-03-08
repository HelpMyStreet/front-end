using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.UserService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Reponses;

namespace HelpMyStreetFE.Services.Users
{
    public interface IUserService
    {
        Task<bool> AddBiography(int userId, string details);
        Task<int> CreateUserAsync(string email, string authId, int referringGroupId, string source);
        Task<int> UpdateUserAsync(User user, CancellationToken cancellationToken);
        Task<User> GetUserByAuthId(string authId);
        Task<User> GetUserAsync(int id, CancellationToken cancellationToken);
        Task CreateUserStepTwoAsync(int id, string postCode, string firstName, string lastName, string addressLine1, string addressLine2, string addressLine3, string locality, string mobile, string otherPhone, System.DateTime dob, CancellationToken cancellationToken);
        Task CreateUserStepThreeAsync(int id, System.Collections.Generic.List<HelpMyStreet.Utils.Enums.SupportActivities> activities, float supportRadius, RegistrationFormVariant registrationFormVariant, CancellationToken cancellationToken);
        Task<int> GetVolunteers();
        Task<GetHelpersByPostcodeResponse> GetHelpersByPostcode(string postcode);
        Task<VolunteerCoordinatesResponse> GetVolunteerCoordinates(double swLatitude, double swLongitude, double neLatitude, double neLongitude, int minDistanceBetweenInMetres);
        Task<Models.Account.UserDetails> GetUserDetails(User user, CancellationToken cancellationToken);
        bool GetRegistrationIsComplete(User user);
    }
}