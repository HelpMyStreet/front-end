using HelpMyStreetFE.Models.Community;
using System.Collections.Generic;

namespace HelpMyStreetFE.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        public CommunityViewModel GetCommunity(string communityName)
        {
            switch (communityName.Trim().ToLower())
            {
                case "tankersley":
                    return GetTankersley();     
                default:
                    return null;
            }
        }
        

        private CommunityViewModel GetTankersley()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            communityViewModel.Latitude = 53.498113;
            communityViewModel.Longitude = -1.488587;
            communityViewModel.ZoomLevel = 14;

            communityViewModel.CommunityName = "Pilley and Tankersley";

            communityViewModel.BannerImageLocation = "/img/community/tankersley/tankersley-st-peters-church.jpeg";

            communityViewModel.LogoImageLocation = @"/img/community/kimberley/kimberley-and-district-logo.png";

            communityViewModel.Header = "Pilley and Tankersley people taking care of each other";

            communityViewModel.CommunityVolunteersHeader = "Welcome from Pilley and Tankersley Community Helpers";

            communityViewModel.CommunityVolunteersTextHtml =
 @"<p>Pilley and Tankersley community helpers are here to help neighbours in need. We can help collecting shopping, running local errands or walking the dog. If you need anything, all you need to do is ask! Request help or sign-up to join us using the buttons below, or call us on XXX.</p>
<p>Did you know... we also have a community fund to help people struggling to buy the essentials. If you need help paying for your shopping please don't go without - call us on XXXX for a confidential chat, we're here to help.</p>
<p>If you want to get in touch, email <a href = ""mailto: tankersley@helpmystreet.org"">tankersley@helpmystreet.org</a></p> 
";

            communityViewModel.RequestHelpHeading = @"How can we help?";

            communityViewModel.RequestHelpText = @"We've got shoppers, showers and hot-meal makers; walkers, talkers and home-work helpers all ready and waiting to help you!";

            communityViewModel.ProvideHelpHeading = "Volunteer with us!";

            communityViewModel.ProvideHelpText = "Join us to help your neighbours. Just let us know when, where and how you can help. You can choose to help a little, or to help a lot! We're grateful for every contribution.";

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Volunteer Name",
                    Role = "Role",
                    Location = "Location",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Volunteer Name",
                    Role = "Role",
                    Location = "Location",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Volunteer Name",
                    Role = "Role",
                    Location = "Location",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Volunteer Name",
                    Role = "Role",
                    Location = "Location",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Volunteer Name",
                    Role = "Role",
                    Location = "Location",
                    ImageLocation = "/img/icons/anonymous-user.png"
                },
            };


            communityViewModel.UsefulLinksHtml = new List<string>()
            {
                @"<a href=""https://www.facebook.com/groups/958956387798343"">Piley & Tankersley Community Page (Facebook Group)</a>",
                @"Piley & Tankersley Community Fund - call XXX on XXX for a confidential chat",
                @"<a href=""https://www.gov.uk/coronavirus-extremely-vulnerable"">GOV.UK - Get coronavirus support as a clinically extremely vulnerable person</a>",
            };


            return communityViewModel;
        }
    }
}
