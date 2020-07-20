using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models.Community;
using HelpMyStreetFE.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        private readonly IGroupService _groupService;

        public CommunityRepository(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<CommunityViewModel> GetCommunity(string communityName)
        {
            switch (communityName.Trim().ToLower())
            {
                case "tankersley":
                    return await GetTankersley();
                case "ageuklsl":
                    return await GetAgeUKLSL();
                case "hlp":
                    return await GetHLP();
                case "ftlos":
                    return await GetFtLOS();
                default:
                    return null;
            }
        }
        
        private async Task<CommunityViewModel> GetHLP()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            var getGroupByKeyResponse = await _groupService.GetGroupByKey("hlp");
            int groupId = getGroupByKeyResponse.GroupId;
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId.ToString());
            communityViewModel.HomeFolder = "hlp";
            communityViewModel.Latitude = 51.507602;
            communityViewModel.Longitude = -0.127816;
            communityViewModel.ZoomLevel = 10;

            communityViewModel.CommunityName = "Healthy London Partnership";
            communityViewModel.CommunityShortName = "Healthy London";

            communityViewModel.BannerImageLocation = "/img/community/hlp/hlp-banner.png";

            communityViewModel.Header = "What are Community Connectors?";
            communityViewModel.DisableButtons = true;
            communityViewModel.HeaderHTML = @"
                    <p class='mt-sm mb-xs'>
                        Mental Health First Aid England is working with the new NHS Connect service to recruit volunteer 
                        Community Connectors as part of the nation-wide response to Covid-19. NHS Connect is a new digital
                        service that helps connect vulnerable people with the support they need.<br>
                    </p>
                    <p class='mt-sm mb-xs'>
                      We are looking for volunteers who combine an understanding of mental health problems with previous training 
                        and experience in one or more of these practical and ethical frameworks: coaching, motivational interviewing,
                        counselling or an accredited form of therapy.
                    </p>
                    <p class='mt-sm mb-xs'>This is your opportunity to sign up as a pioneer volunteer.</p>
                   <div class='input'>
                        <p class='mb-xs mt-sm'>Please confirm that:</p>
                       <form>
                            <div class='input input--checkbox'>
                                <label class='small'>
                                    <input type='checkbox' autocomplete='off' class='select-all' hidden />
                                    <span class='input--checkbox__checkbox'>
                                        <span class='mdi mdi-check'></span>
                                    </span>
                                    You pledge to commit 3-4 hours or your time per week (although no two weeks are alike) 
                                </label>
                            </div>
                            <div class='input input--checkbox'>
                                <label class='small'>
                                    <input type='checkbox' autocomplete='off' class='select-all' hidden />
                                    <span class='input--checkbox__checkbox'>
                                        <span class='mdi mdi-check'></span>
                                    </span>
                                    You have previous training and experience in one or more of these practical and ethical frameworks (coaching; motivational interviewing; counselling, or an accredited form of therapy)
                                </label>
                            </div>
                       </form>
                    </div>";                        
                        
                            
            communityViewModel.ShowRequestHelp = false;
            communityViewModel.HeaderVolunteerButtonText = null;
            communityViewModel.ShowHelpExampleCards = false;
            communityViewModel.CommunityVolunteersHeader = "Welcome from Healthy London Partnership";



            communityViewModel.CommunityVolunteersTextHtml =
            @"<p>We aim to make London the healthiest global city by working with our partners to improve Londoners' health and wellbeing so everyone can live healthier lives.</p>
            <p>Our partners include the NHS in London (Clinical Commissioning Groups, Health Education England, NHS England, NHS Digital, NHS Improvement, trusts and providers), the Greater London Authority, the Mayor of 
            London, Public Health England and London Councils.</p>";

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


            communityViewModel.UsefulLinksHtml =
                @"<p><a href=""https://www.healthylondon.org/"">Healthy London Partnership</a> - for more information on the work we do.</p>
                  <p><a href=""https://mhfaengland.org/"">MHFA England</a> - to learn more about mental health training</p>";


            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetTankersley()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            var getGroupByKeyResponse = await _groupService.GetGroupByKey("tankersley");
            int groupId = getGroupByKeyResponse.GroupId;
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId.ToString());
            communityViewModel.HomeFolder = "tankersley";
            communityViewModel.Latitude = 53.498113;
            communityViewModel.Longitude = -1.488587;
            communityViewModel.ZoomLevel = 14;

            communityViewModel.CommunityName = "Tankersley & Pilley";

            communityViewModel.BannerImageLocation = "/img/community/tankersley/tankersley-st-peters-church.jpeg";

            communityViewModel.Header = "In Tankersley & Pilley, help is always available!";

            communityViewModel.CommunityVolunteersHeader = "Welcome from Tankersley & Pilley Community Helpers";
            communityViewModel.HeaderHTML = @"
                    <p class='mt-sm mb-xs'>
                        In our community there’s always somebody here to help, there’s no need for anyone to struggle alone.
                        We’re the Tankersley &amp; Pilley Community Helpers, here to help with:
                    </p>
                    <ul class='tick-list mt-xs mb-sm compact-list'>
                        <li>Shopping for essentials</li>
                        <li>A friendly chat</li>
                        <li>Help at home</li>
                        <li>Cooking a hot meal</li>
                    </ul>
                    ";



            communityViewModel.CommunityVolunteersTextHtml =
 @"<p>Pilley and Tankersley community helpers are here to help neighbours in need. We can help collecting shopping, running local errands or walking the dog.</p>
<p>To join us or to get in touch, email <a href = ""mailto: tankersley@helpmystreet.org"">tankersley@helpmystreet.org</a></p> 
";

            communityViewModel.RequestHelpHeading = @"How can we help?";

            communityViewModel.RequestHelpText = @"We've got shoppers, sewers and hot-meal makers; walkers, talkers and home-work helpers all ready and waiting to help you!";

            communityViewModel.ProvideHelpHeading = "Volunteer with us!";

            communityViewModel.ProvideHelpText = "Join us to help your neighbours. Just let us know when, where and how you can help. You can choose to help a little, or to help a lot! We're grateful for every contribution.";

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "David",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/tankersley/David.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Kate",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/tankersley/Kate.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Helen",
                    Role = "",
                    Location = "",
                    ImageLocation = "/img/community/tankersley/Helen.jpg"
                },
            };


            communityViewModel.UsefulLinksHtml = @"<p><a href=""https://www.facebook.com/groups/958956387798343"">Piley & Tankersley Community Page (Facebook Group)</a></p>";


            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetAgeUKLSL()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            var getGroupByKeyResponse = await _groupService.GetGroupByKey("ageuklsl");
            int groupId = getGroupByKeyResponse.GroupId;
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId.ToString());
            communityViewModel.HomeFolder = "ageUK";
            communityViewModel.Latitude = 52.95;
            communityViewModel.Longitude = -0.2;
            communityViewModel.ZoomLevel = 9;

            communityViewModel.CommunityName = "Age UK Lincoln (& SL)";
            communityViewModel.CommunityShortName = "Age UK LSL";

            communityViewModel.BannerImageLocation = "/img/community/ageUK/ageUKlogo.png";

            communityViewModel.Header = "Veterans need your help!";
            communityViewModel.HeaderHTML = @"
                    <p class='mt-sm mb-xs'>
                        We are now underway with our new “Vitals For Veterans” project.
                    </p>
                    <p class='mt-sm mb-xs'>
                        Over the coming months, we are aiming to deliver wellbeing packs to veterans across Lincoln & South Lincolnshire because
                        <i>veterans should not be forgotten</i>.
                    </p>
                    <p class='mt-sm mb-s'>
                        If you can help us deliver wellbeing packs to veterans in your area please sign up below.
                    </p>";
            communityViewModel.CommunityVolunteersHeader = "Welcome from Age UK Lincoln and South Lincolnshire";
            communityViewModel.HeaderVolunteerButtonText = null;



            communityViewModel.CommunityVolunteersTextHtml =
                 @"<p>
                    Age UK Lincoln & South Lincolnshire is an independent, local charity and we have been working in the local community to help older people,
                    their families and carers for over 61 years. We have over 400 dedicated staff and volunteers helping us to deliver services and activities
                    for older people in Lincoln and across the county.
                </p>
                <p>
                    Supporting over 4,000 people aged 50 and over every week, our dedicated staff and volunteers deliver support services and activities across
                    17 departments for people countywide.
                </p>
                <p>
                    Our support is varied and extensive; 11,845 people attended activities with us in 2018/19,  helping to combat social isolation by offering
                    opportunities for engagement through social activities, clubs and groups. Age UK Lincoln & South Lincolnshire exists to improve the lives of
                    older people, through supporting them to love later life and helping them where possible to remain independent in their own homes. We
                    continually work towards ending loneliness and isolation in older people, many of whom were isolated and living in a form of lockdown before
                    the recent pandemic and sadly for whom there is no “new normal”. Their reality remains loneliness and isolation.
                </p>
                <p>
                    Following a successful application to the Armed Forces Covenant Fund Age UK Lincoln & South Lincolnshire have been awarded some funding to
                    deliver Vitals for Veterans because “Veterans should not be forgotten”. With the funding and additional support from local businesses offering
                    generous donations of funds and items we are able to deliver free packs to veterans across Lincoln & South Lincolnshire between June and November.
                </p>
                <p>
                    For more information on our services and support, please get in touch by email <a href = ""mailto:info@ageuklsl.org.uk"">info@ageuklsl.org.uk</a>
                    or call us on 03455 564 144.
                </p> 
                ";
            communityViewModel.ShowRequestHelp = false;

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Nicki Lee",
                    Role = "Senior Volunteer Coordinator",
                    Location = "Lincoln & South Lincolnshire",
                    ImageLocation = "/img/community/ageuk/NL_cropped.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Amanda Wilson",
                    Role = "Engagement Coordinator",
                    Location = "Lincoln & South Lincolnshire",
                    ImageLocation = "/img/community/ageuk/AW_cropped.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Melanie Meik",
                    Role = "Fundraising & Marketing Manager",
                    Location = "Lincoln & South Lincolnshire",
                    ImageLocation = "/img/community/ageuk/MM_cropped.jpg"
                },
            };


            communityViewModel.UsefulLinksHtml = 
                @"<p><a href=""https://www.ageuk.org.uk/lincolnsouthlincolnshire/activities-and-events/vitals-for-veterans"">Vitals for Veterans page</a> - Find out more about our incredible project reaching veterans in need across Lincoln and South Lincolnshire</p>
                <p><a href=""https://www.ageuk.org.uk/lincolnsouthlincolnshire"">Age UK Lincoln and South Lincolnshire main site</a> - Find out more about Age UK Lincoln and South Lincolnshire services</p>
                <p><a href=""https://www.justgiving.com/age-uk-lincoln"">Our Just Giving site</a> - Donate to help older people in Lincoln and South Lincolnshire</p>";


            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetFtLOS()
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            var getGroupByKeyResponse = await _groupService.GetGroupByKey("ftlos");
            int groupId = getGroupByKeyResponse.GroupId;
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId.ToString());
            communityViewModel.HomeFolder = "fortheloveofscrubs";
            communityViewModel.CommunityName = "For the Love of Scrubs";

            communityViewModel.BannerImageLocation = "/img/community/fortheloveofscrubs/LOSBannerWide.png";

            communityViewModel.ShowMap = false;

            communityViewModel.Header = "A message from Ashleigh Linsdell, Founder and Director of ‘For the Love of Scrubs’ (FTLOS)";
            communityViewModel.HeaderHTML = @"
                <div class='row sm8' style='padding-left: 0'>    
                    <p class='mt-sm mb-sm'>
                        FTLOS has supplied many thousands of items to support frontline workers in hospitals, care homes and surgeries across the
                        country. We now have another line of defence to support - our communities.
                    </p>
                    <p class='mt-sm mb-sm'>
                        We need everyone to do their bit to protect communities against coronavirus by wearing a fabric face covering when out and
                        about in shops, buses, trains and other public places. Wearing a fabric face covering helps keep everyone safe and supports
                        the national defence strategy against the spread of COVID-19.
                    </p>
                    <p class='mt-sm mb-sm'>
                        Everyone is asked to donate £3 to £4 per face covering to cover the cost of materials and to help us support our communities.
                        You’ll also be asked to cover the cost of postage (if needed).
                    </p>
                    <p class='mt-sm mb-md'>
                        If you’d like to get involved, sign up to sew or request a face covering now.
                    </p>
                </div>
                <div class='row sm4 mt-sm mb-md'>
                    <div class='gfm-embed' data-url='https://www.gofundme.com/f/for-the-love-of-scrubs-face-coverings/widget/medium'></div>
                </div>
                <script defer src='https://www.gofundme.com/static/js/embed.js'></script>
                    ";

            communityViewModel.HeaderButtonWidth = 4;
            communityViewModel.HeaderPostButtonsHTML = @"<div class='row sm4 large-screen-only'></div>";

            communityViewModel.HeaderHelpButtonText = null;
            communityViewModel.HeaderVolunteerButtonText = null;


            communityViewModel.CommunityVolunteersHeader = "What to expect from FTLOS";

            communityViewModel.CommunityVolunteersTextHtml =
            @"
                        <p>
                            The face coverings made by FTLOS volunteers are reusable, washable and well constructed. They can be posted straight to your home - we’ll just ask you to cover the cost of postage before we send them.
                        </p>
                        <p class='mb-xs mt-md'>
                            <strong>Is there a cost for face coverings?</strong>
                            On top of the postage, all we ask if for a small donation of £3 - £4 per face covering to cover the cost of materials and to help us support our communities. You can donate through our GoFundMe page which is linked at the top of this page. At the same time, you can also donate to support the free supply of items we’re making for the NHS and our communities:
                        </p>
                        <p class='mt-0 mb-0 ml-xl'>
                            £10 pays for a metre of fabric we can use for scrubs and other NHS supplies
                        </p>
                        <p class='mt-0 mb-0 ml-xl'>
                            £25 pays for enough face coverings to help a small care home receive visitors
                        </p>
                        <p class='mt-0 mb-0 ml-xl'>
                            £50 pays for a full set of scrubs for an NHS nurse or doctor
                        </p>
                        <p class='mt-md'>
                            <strong>Who can order face coverings?</strong>
                            FTLOS face coverings are made by our communities for our communities, so we can’t accept commercial orders. Items supplied by our volunteers cannot be offered for resale under any circumstances.
                        </p>
                        <p class='mt-md'>
                            <strong>Will all of your face coverings look the same?</strong>
                            Hopefully not! We use a few tried and tested patterns but lots of different fabrics.
                        </p>
                        <p class='mt-md'>
                            <strong>How can I volunteer with For the Love of Scrubs?</strong>
                            Join us by signing up to sew. Our sewers receive a pre-prepared pack of materials straight to their door, once it has arrived you’ll have all you need to get sewing!
                        </p>
                        <p class='mt-md'>
                            <strong>How can I donate to help you continue the good work?</strong>
                            You can donate through For the Love of Scrubs’ <a href='https://www.gofundme.com/f/for-the-love-of-scrubs-face-coverings'>gofundme page</a>.
                        </p>
            ";

            communityViewModel.UsefulLinksHtml = @"
                    <h6>Patterns</h6>
                    <p class='mb-xs'><a href='https://sustainmycrafthabit.com/how-to-make-a-simple-pleated-face-mask-with-free-pattern/#:~:text=%20Instructions%20%201%20Lay%20fabric%20out%20on,around%20the%20perimeter%20and%20stitch%20the...%20More%20'> Simple Pleated Face Mask</a> - free pattern and video</p>
                    <p class='mt-xs mb-xs'><a href='https://hellosewing.com/wp-content/uploads/face-mask-pattern-1.jpg'>Fitted face mask pattern</a> - free pattern</p>
                    <p class='mt-xs mb-xs'><a href='https://www.youtube.com/watch?v=b78VGVWa6hw'>Smile mask (with a clear panel)</a> - YouTube video</p>
                    <p class='mt-xs'><a href='https://freeprintablesonline.com/2020/03/printable-face-mask-patterns-roundup/'> FreePrintablesOnline.com</a> - patterns, videos, hints and tips</p>

                    <h6>Donation page</h6>
                    <p>You can donate through For the Love of Scrubs’ <a href='https://www.gofundme.com/f/for-the-love-of-scrubs-face-coverings'>gofundme page</a></p>

                    <h6>Requesting materials</h6>
                    <p>If you’re a FTLOS sewer and need more materials contact your local group administrator or email <a href='mailto:materialrequest-ftlos@outlook.com'>materialrequest-ftlos@outlook.com</a></p>
            ";

            communityViewModel.RequestHelpHeading = "Do you need a face covering?";
            communityViewModel.RequestHelpText = "If you’d like some For the Love of Scrubs face coverings for yourself, your family or an organisation, request them now. We'll do our best to help! ";
            communityViewModel.RequestHelpButtonText = "Request a Face Covering";

            communityViewModel.ProvideHelpHeading = "Volunteer with us!";
            communityViewModel.ProvideHelpText = "If you’d like to join For the Love of Scrubs (or register as an existing member) sign up now. We’ll send you everything you need to get started (except for the sewing machine!)";
            communityViewModel.ProvideHelpLoggedOutButtonText = "Sign Up to Sew";

            communityViewModel.HelpExampleCards = new Models.HelpExampleCardsViewModel()
            {
                Example1 = "6 plain face coverings to safely commute",
                Example2 = "Bright face coverings to cheer my mum up in hospital",
                Example3 = "10 donated face coverings for care home visitors"
            };


            return communityViewModel;
        }
    }
}
