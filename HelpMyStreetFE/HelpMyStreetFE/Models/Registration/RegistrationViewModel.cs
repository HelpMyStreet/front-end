using HelpMyStreetFE.Models.RequestHelp.Enum;

namespace HelpMyStreetFE.Models.Registration
{
    public class RegistrationViewModel
    {
        public int ActiveStep { get; set; }
        public string FirebaseConfiguration { get; set; }
        public string EncodedUserID { get; set; }
        public int ReferringGroupID { get; set; }
        public RegistrationSource Source { get; set; }
    }
}
