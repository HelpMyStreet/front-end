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
        public string HeaderSubtitle { get; set; } = "HelpMyStreet connects neighbours in need with people who can help -safely and securely.";
        public IEnumerable<string> HeaderBullets { get; set; } = new List<string> { "Shopping for essentials", "Cooking a hot meal", "Home-sewn face coverings", "Help with home-schooling", "A friendly chat" };

        public string HeaderHelpButtonText { get; set; } = "It's OK to ask for a little help";
        public string HeaderVolunteerButtonText { get; set; } = "Help your neighbours in need";

        public bool HideHeaderButtons { get; set; } = false;
        public bool HideHeaderVolunteerButton { get; set; } = false;
        public bool HideHeaderHelpButton { get; set; } = false;

        public string CommunityVolunteersHeader { get; set; }
        public string CommunityVolunteersTextHtml { get; set; }

        public bool HideHelpPanel { get; set; } = false;
        public string RequestHelpHeading { get; set; }
        public string RequestHelpText { get; set; }

        public string ProvideHelpHeading { get; set; }
        public string ProvideHelpText { get; set; }

        public IEnumerable<CommunityVolunteer> CommunityVolunteers { get; set; }
        public IEnumerable<string> UsefulLinksHtml { get; set; }
            }
}
