using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Community;

namespace HelpMyStreetFE.Controllers
{
    public class CommunityController : Controller
    {
        private readonly ILogger<CommunityController> _logger;

        public CommunityController(ILogger<CommunityController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index(string community)
        {
            var communityViewModel = new CommunityViewModel();

            communityViewModel.IsLoggedIn = ((HttpContext.User != null) && HttpContext.User.Identity.IsAuthenticated);

            communityViewModel.CommunityName = "Kimberley and District";

            communityViewModel.LogoImageLocation = @"/img/community/kimberley/kimberley-and-district-logo.png";

            communityViewModel.TopPanelHeader = "Kimberley people taking care of each other";

            communityViewModel.CommunityVolunteersHeader = "Welcome from Kimberley and District Mutual Aid";

            communityViewModel.CommunityVolunteersText = @"Kimberley and District Mutual Aid has been set to help those living in Kimberley, Swingate, Nuthall and Watnall find help from their neighbours during the Coronavirus outbreak. Our volunteers are happy to help wherever they can, including help collecting shopping, running local errands or walking the dog. If you need anything, get in touch!

Kimberley and District Mutual Aid has been set to help those living in Kimberley, Swingate, Nuthall and Watnall find help from their neighbours during the Coronavirus outbreak. Our volunteers are happy to help wherever they can, including help collecting shopping, running local errands or walking the dog. If you need anything, get in touch!

Kimberley and District Mutual Aid has been set to help those living in Kimberley, Swingate, Nuthall and Watnall find help from their neighbours during the Coronavirus outbreak. Our volunteers are happy to help wherever they can, including help collecting shopping, running local errands or walking the dog. If you need anything, get in touch!
";

            communityViewModel.RequestHelpHeading = @"How can we help?";

            communityViewModel.RequestHelpText = @"We’ve got shoppers, sewers and hot-meal makers; walkers, talkers and home-work helpers. All ready and waiting to help you!";

            communityViewModel.ProvideHelpHeading = "Join the Kimberley and District Volunteers!";

            communityViewModel.ProvideHelpText = "And help people stay safe. Just let us know when, where and how you can help. You can choose to help a little, or to help a lot! We’re grateful for every contribution.";

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Laura Kimberly",
                    Title = "Community Leader",
                    ImageLocation = "https://picsum.photos/300/300?1"
                },
                new CommunityVolunteer()
                {
                    Name = "Sam Swingate",
                    Title = "Street Champion",
                    ImageLocation = "https://picsum.photos/250/300?2"
                },
                new CommunityVolunteer()
                {
                    Name = "Cara Nuthall",
                    Title = "Street Champion",
                    ImageLocation = "https://picsum.photos/250/250?3"
                },
                new CommunityVolunteer()
                {
                    Name = "John Watnall",
                    Title = "Helper",
                    ImageLocation = "https://picsum.photos/350/250?4"
                },
                new CommunityVolunteer()
                {
                    Name = "Tom Kimberly",
                    Title = "Helper",
                    ImageLocation = "https://picsum.photos/200/300?5"
                },
            };

            return View(communityViewModel);
        }

        
    }
}
