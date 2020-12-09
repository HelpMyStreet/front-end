using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.Collections.Generic;
using HelpMyStreetFE.Models.Feedback;

namespace HelpMyStreetFE.Models.Community
{
    public class CommunityViewModel
    {
        public string View { get; set; } = "Index";
        public bool IsLoggedIn { get; set; }
        public bool IsGroupMember { get; set; }
        public string EncodedGroupId { get; set; }
        public bool ShowMap { get; set; } = true;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double ZoomLevel { get; set; }
        public int HeaderButtonWidth { get; set; } = 6;
        public string HeaderPostButtonsHTML { get; set; }
        public string CommunityName { get; set; }
        public string CommunityShortName { get; set; }
        public string BannerImageLocation { get; set; }

        public string Header { get; set; }
        public string HeaderHelpButtonText { get; set; } = "It's OK to ask for a little help";
        public string HeaderVolunteerButtonText { get; set; } = "Help your neighbours in need";

        public string HeaderHTML { get; set; }
        public string CommunityVolunteersHeader { get; set; }
        public string CommunityVolunteersTextHtml { get; set; }

        public bool ShowRequestHelp { get; set; } = true;
        public bool ShowHelpExampleCards {get;set;} = false;
        public HelpExampleCardsViewModel HelpExampleCards { get; set; } = new HelpExampleCardsViewModel();

        public bool DisableButtons { get; set; }
        public string RequestHelpHeading { get; set; }
        public string RequestHelpText { get; set; }
        public string RequestHelpButtonText { get; set; } = "Request Help";
        public string ProvideHelpButtonText_LoggedOut { get; set; } = "Sign Up or Log In";
        public string ProvideHelpButtonText_LoggedIn { get; set; } = "View Open Requests";

        public string ProvideHelpHeading { get; set; }
        public string ProvideHelpText_NotGroupMember { get; set; }
        public string ProvideHelpText_GroupMember { get; set; }
        public IEnumerable<CommunityVolunteer> CommunityVolunteers { get; set; }
        public string UsefulLinksHtml { get; set; }
        public IEnumerable<string> CarouselImages1 { get; set; }
        public IEnumerable<string> CarouselImages2 { get; set; }
        public IEnumerable<string> CarouselImages3 { get; set; }

        public FeedbackMessageType showFeedbackType { get; set; } = FeedbackMessageType.Other;
        public string groupKey { get; set; }
        public bool ShowRequestHelpPopup { get; set; }

        public bool AllowJoinOurGroup { get; set; }
        public bool AllowLeaveOurGroup { get; set; }
        public string JoinOurGroupButtonText { get; set; }

        public bool ShowVisitWebsite { get; set; }
        public string VisitWebsiteHeading { get; set; }
        public string VisitWebsiteText { get; set; }
        public string VisitWebsiteButtonText { get; set; }
        public string WebsiteUrl { get; set; }
        public string HeaderVisitWebsiteButtonText { get; set; }
    }
}
