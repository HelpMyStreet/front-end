using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Community
{
    public class CommunityViewModel
    {
        public bool IsLoggedIn{ get; set; }
        public string CommunityName { get; set; }
        public string LogoImageLocation { get; set; }
        public string ImageLocation { get; set; }
        public string TopPanelHeader { get; set; }

        public string CommunityVolunteersHeader { get; set; }
        public string CommunityVolunteersText { get; set; }


        public string RequestHelpHeading { get; set; }
        public string RequestHelpText { get; set; }

        public string ProvideHelpHeading { get; set; }
        public string ProvideHelpText { get; set; }

        public IEnumerable<CommunityVolunteer> CommunityVolunteers { get; set; }
        public IEnumerable<string> UsefulLinks { get; set; }
    }
}
