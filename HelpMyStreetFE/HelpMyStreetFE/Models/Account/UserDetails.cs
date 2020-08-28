using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account
{
    public class UserDetails
    {
        public UserDetails(User user)
        {
            User = user;
        }

        public User User { get; set; }
        public string Initials
        {
            get
            {
                if (User?.UserPersonalDetails == null || string.IsNullOrEmpty(User.UserPersonalDetails.FirstName))
                {
                    return "??";
                }
                return User.UserPersonalDetails.FirstName.Substring(0, 1).ToUpper() + User.UserPersonalDetails.LastName.Substring(0, 1).ToUpper();
            }
        }
        public string DisplayName { get { return User?.UserPersonalDetails?.DisplayName ?? "??"; } }
        public string FirstName { get { return User?.UserPersonalDetails?.FirstName ?? "Not Set"; } }
        public string LastName { get { return User?.UserPersonalDetails?.LastName ?? "Not Set"; } }
        public string EmailAddress { get { return User?.UserPersonalDetails?.EmailAddress ?? "Not Set"; } }
        public string Address { get
            {
                if (User?.UserPersonalDetails == null || string.IsNullOrEmpty(User.UserPersonalDetails.Address?.AddressLine1))
                {
                    return "Not Set";
                }
                return string.Join(", ", new[] { User.UserPersonalDetails.Address.AddressLine1, User.UserPersonalDetails.Address.Postcode });
            }
        }
        public string MobileNumber { get { return User?.UserPersonalDetails?.MobilePhone ?? "Not Set"; } }
        public string OtherNumber { get { return User?.UserPersonalDetails?.OtherPhone ?? "Not Set"; } }
        public string DateOfBirth { get { return User?.UserPersonalDetails?.DateOfBirth?.ToString("dd/MM/yyyy") ?? "Not Set"; } }
        public string StreetChampion { get { return IsStreetChampion ? "Street Champion" : "Helper"; } }
        public List<string> ChampionPostcodes { get { return User?.ChampionPostcodes ?? new List<string>(); } }
        public bool IsStreetChampion { get { return User != null && User.StreetChampionRoleUnderstood.HasValue && User.StreetChampionRoleUnderstood.Value; } }
        public bool IsVerified { get { return User?.IsVerified ?? false; } }

    }
}
