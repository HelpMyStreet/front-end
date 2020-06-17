using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Community
{
    public class CommunityViewModel
    {
        public bool IsLoggedIn { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ZoomLevel { get; set; }

        public string CommunityName { get; set; }

        public string BannerImageLocation { get; set; }

        public string Header { get; set; }
        public string HeaderHTML { get; set; }
        public string CommunityVolunteersHeader { get; set; }
        public string CommunityVolunteersTextHtml { get; set; }

        public bool ShowRequestHelp { get; set; } = true;
        public bool ShowHelpExampleCards {get;set;} = true;
        public string RequestHelpHeading { get; set; }
        public string RequestHelpText { get; set; }

        public string ProvideHelpHeading { get; set; }
        public string ProvideHelpText { get; set; }

        public IEnumerable<CommunityVolunteer> CommunityVolunteers { get; set; }
        public IEnumerable<string> UsefulLinksHtml { get; set; }
    }
}
