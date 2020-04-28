using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account
{
    public class StreetsViewModel
    {
        public StreetsViewModel()
        {
            Streets = new List<Street>();
        }
     public List<Street> Streets { get; set; } 
    }

    public class Street
    {
        public Street()
        {
            Helpers = new List<Helper>();
        }
        public string Name { get; set; }
        public string FriendlyName { get; set; }
        public List<Helper> Helpers { get; set; }
    }

    public class Helper
    {
        public bool IsStreetChampion { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AlternatePhoneNumber { get; set; }
        public List<HelpMyStreet.Utils.Enums.SupportActivities> SupportedActivites { get; set; }
    }
}
