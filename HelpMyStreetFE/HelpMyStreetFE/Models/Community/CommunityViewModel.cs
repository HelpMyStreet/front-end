﻿using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using HelpMyStreetFE.Models.Feedback;

namespace HelpMyStreetFE.Models.Community
{
    public class CommunityViewModel
    {
        public bool IsLoggedIn { get; set; }
        public bool IsGroupMember { get; set; }

        public string View { get; set; }
        public string EncodedGroupId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double ZoomLevel { get; set; }

        public string CommunityName { get; set; }
        public string CommunityShortName { get; set; }

        public HelpExampleCardsViewModel HelpExampleCards { get; set; } = new HelpExampleCardsViewModel();

        public IEnumerable<CommunityVolunteer> CommunityVolunteers { get; set; }

        public List<List<string>> CarouselImages { get; set; }

        public string groupKey { get; set; }
        public bool ShowRequestHelpPopup { get; set; }
    }
}
