using System.Collections.Generic;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp.Enum;

namespace HelpMyStreetFE.Models.Registration
{
    public class RegistrationViewModel
    {
        public int ActiveStep { get; set; }
        public string FirebaseConfiguration { get; set; }
        public int ReferringGroupID { get; set; }
        public string Source { get; set; }
        public RegistrationFormVariant RegistrationFormVariant { get; set; }
        public string GroupName { get; set; }
        public string Email { get; set; }
        public List<SupportActivityViewModel> ActivityDetails { get; set; }
    }
}
