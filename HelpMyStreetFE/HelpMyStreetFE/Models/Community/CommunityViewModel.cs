using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Community
{
    public class CommunityViewModel
    {
        public bool IsLoggedIn { get; set; }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ZoomLevel { get; set; }
        public string SignUpLink { get; set; } = "/registration/stepone";
        public string CommunityName { get; set; }
        public string CommunityShortName { get; set; }
        public string BannerImageLocation { get; set; }

        public string Header { get; set; }
        public string HeaderHTML { get; set; } = GetDefaultHeaderHtml();
        public string CommunityVolunteersHeader { get; set; }
        public string CommunityVolunteersTextHtml { get; set; }

        public bool ShowRequestHelp { get; set; } = true;
        public bool ShowHelpExampleCards {get;set;} = true;
        public bool DisableButtons { get; set; }
        public string RequestHelpHeading { get; set; }
        public string RequestHelpText { get; set; }

        public string ProvideHelpHeading { get; set; }
        public string ProvideHelpText { get; set; }
        public IEnumerable<CommunityVolunteer> CommunityVolunteers { get; set; }
        public IEnumerable<string> UsefulLinksHtml { get; set; }


        private static string GetDefaultHeaderHtml()
        {
            return @"
                    <p class='row sm12 text-left mt- sm mb-xs'>
                        HelpMyStreet connects neighbours in need with people who can help safely and securely.<br>
                    </p>
                   <div class='row sm12'>
                        <ul class='tick-list mt-xs mb-sm compact-list'>
                            <li>Shopping for essentials</li>
                            <li>Cooking a hot meal</li>
                            <li>Home-sewn face coverings</li>
                            <li>Help with home-schooling</li>
                            <li>A friendly chat</li>
                        </ul>
                    </div>
                    <p class='row sm12 mt-sm mb-sm'>It's OK to ask for a little help</p>
                    <div class='row sm12 text-center justify-center small-screen-only mb-sm'>
                        <a href='/request-help' class='btn cta small btn--request-help cta--orange'>Request Help</a>
                    </div>
                    <p class='row sm12 mt-sm mb-sm'>Help your neighbours in need</p>";
        }

    }
}
