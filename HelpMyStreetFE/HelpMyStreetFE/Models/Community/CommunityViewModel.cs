using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Utils.Utils;
using HelpMyStreet.Contracts;

namespace HelpMyStreetFE.Models.Community
{
    public class CommunityViewModel
    {
        public bool IsLoggedIn { get; set; }
        public bool IsGroupMember { get; set; }

        public string View { get; set; }
        public double Map_CentreLatitude { get; set; }
        public double Map_CentreLongitude { get; set; }
        public double Map_ZoomLevel { get; set; }

        public string CommunityName { get; set; }
        public string CommunityShortName { get; set; }

        public HelpExampleCardsViewModel HelpExampleCards { get; set; }

        public IEnumerable<CommunityVolunteer> CommunityVolunteers { get; set; }

        public List<List<string>> CarouselImages { get; set; }

        public Group Group { get; set; }
        public string EncodedGroupId
        {
            get
            {
                return Base64Utils.Base64Encode(Group.GroupId);
            }
        }

        public bool ShowRequestHelpPopup { get; set; }
        public bool ShowPopupOnSignUp { get; set; } = false;

        public IEnumerable<NewsTickerMessage> NewsTickerMessages { get; set; }
    }
}
