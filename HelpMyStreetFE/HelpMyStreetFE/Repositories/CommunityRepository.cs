using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models.Community;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using System.Linq;
using HelpMyStreetFE.Services.Groups;

namespace HelpMyStreetFE.Repositories
{
    public class CommunityRepository : ICommunityRepository
    {
        private readonly IGroupService _groupService;
        private Dictionary<string, CommunityModel> Communities = new Dictionary<string, CommunityModel>()
        {
            {"hlp", new CommunityModel(){FriendlyName = "Healthy London Partnership", Latitude = 51.507602, Longitude = -0.127816, ReferenceName = "hlp", LinkURL = "/healthylondonpartnership", ZoomLevel = 10, DisplayOnMap = false, BannerLocation = "/img/community/hlp/hlp-banner.png"} },
            {"tankersley", new CommunityModel(){FriendlyName = "Tankersley & Pilley Community Helpers", Latitude = 53.498113, Longitude = -1.488587, ReferenceName = "tankersley", LinkURL = "/tankersley", ZoomLevel = 14, BannerLocation = "/img/community/tankersley/tankersley-st-peters-church.jpeg" } },
            {"ruddington", new CommunityModel(){FriendlyName = "Ruddington Community Response Team", Latitude = 52.8925, Longitude = -1.150, ReferenceName = "ruddington", LinkURL = "/ruddington", ZoomLevel = 14.6, BannerLocation = "/img/community/ruddington/banner.jpg"} },
            {"ageuklsl", new CommunityModel() {FriendlyName = "Age UK Lincoln & South Lincolnshire", Latitude = 53.2304334, Longitude = -0.5435425, ReferenceName = "ageuklsl", LinkURL = "/ageuklsl", ZoomLevel = 9, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/ageUKlogo.png"} },
            {"ageukwirral", new CommunityModel() {FriendlyName = "Age UK Wirral", Latitude = 53.397320, Longitude = -3.042670, ReferenceName = "ageukwirral", LinkURL = "/ageukwirral", ZoomLevel = 9, DisplayOnMap = true, BannerLocation = "/img/community/ageUK/wirral/age-uk-wirral-banner.png"} }
        };

        public CommunityRepository(IGroupService groupService)
        {
            _groupService = groupService;
        }

        public async Task<CommunityViewModel> GetCommunity(string communityName, CancellationToken cancellationToken)
        {
            switch (communityName.Trim().ToLower())
            {
                case "tankersley":
                    return await GetTankersley(cancellationToken);
                case "ruddington":
                    return await GetRuddington(cancellationToken);
                case "ageuklsl":
                    return await GetAgeUKLSL(cancellationToken);
                case "ageukwirral":
                    return await GetAgeUKWirral(cancellationToken);
                case "hlp":
                    return await GetHLP(cancellationToken);
                case "ftlos":
                    return await GetFtLOS(cancellationToken);
                default:
                    return null;
            }
        }

        public async Task<List<CommunityModel>> GetCommunities()
        {
            List<CommunityModel> returnCommunities = new List<CommunityModel>();
            foreach (var item in Communities){
                returnCommunities.Add(item.Value);
            }
            return returnCommunities;

        }

        public async Task<CommunityModel> GetCommunityDetailByKey(string key)
        {
            return Communities[key];
        }
        
        private async Task<CommunityViewModel> GetHLP(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();
            CommunityModel communityModel = await GetCommunityDetailByKey("hlp");

            int groupId = await _groupService.GetGroupIdByKey("hlp", cancellationToken);

            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.HomeFolder = "hlp";
            communityViewModel.Latitude = communityModel.Latitude;
            communityViewModel.Longitude = communityModel.Longitude;
            communityViewModel.ZoomLevel = communityModel.ZoomLevel;

            communityViewModel.CommunityName = communityModel.FriendlyName;
            communityViewModel.CommunityShortName = "Healthy London";

            communityViewModel.BannerImageLocation = communityModel.BannerLocation;

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
            @"<p>We aim to make London the healthiest global city by working with our partners to improve Londoners’ health and wellbeing so everyone can live healthier lives.</p>
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

        private async Task<CommunityViewModel> GetTankersley(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();
            CommunityModel communityModel = await GetCommunityDetailByKey("tankersley");

            int groupId = await _groupService.GetGroupIdByKey("tankersley", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.HomeFolder = "tankersley";
            communityViewModel.Latitude = communityModel.Latitude;
            communityViewModel.Longitude = communityModel.Longitude;
            communityViewModel.ZoomLevel = communityModel.ZoomLevel;

            communityViewModel.showFeedbackType = Models.Feedback.FeedbackMessageType.Group;
            communityViewModel.groupKey = "tankersley";

            communityViewModel.CommunityName = communityModel.FriendlyName;

            communityViewModel.BannerImageLocation = communityModel.BannerLocation;

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

            communityViewModel.RequestHelpText = @"We’ve got shoppers, sewers and hot-meal makers; walkers, talkers and home-work helpers all ready and waiting to help you!";

            communityViewModel.ProvideHelpHeading = "Volunteer with us!";

            communityViewModel.ProvideHelpText_NotGroupMember = "Join us to help your neighbours. Just let us know when, where and how you can help. You can choose to help a little, or to help a lot! We’re grateful for every contribution.";
            communityViewModel.ProvideHelpText_GroupMember = "Thanks for being part of Tankersley &amp; Pilley Community Helpers.  Click below to view help requests in your area.";

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

            communityViewModel.AllowJoinOurGroup = true;
            communityViewModel.JoinOurGroupButtonText = "Join Our Group";
            communityViewModel.JoinGroupPopupText = "<p>Would you like to join <b>Tankersley &amp; Pilley Community Helpers</b>?</p>";

            communityViewModel.AllowLeaveOurGroup = true;
            communityViewModel.LeaveGroupPopupText = "<p>Are you sure you want to leave <b>Tankersley &amp; Pilley Community Helpers</b>?</p>";

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetRuddington(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();
            CommunityModel communityModel = await GetCommunityDetailByKey("ruddington");

            communityViewModel.showFeedbackType = Models.Feedback.FeedbackMessageType.Group;
            communityViewModel.groupKey = "ruddington";
            int groupId = await _groupService.GetGroupIdByKey(communityViewModel.groupKey, cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.HomeFolder = "ruddington";
            communityViewModel.Latitude = communityModel.Latitude;
            communityViewModel.Longitude = communityModel.Longitude;
            communityViewModel.ZoomLevel = communityModel.ZoomLevel;

            communityViewModel.ShowHelpExampleCards = false;

            communityViewModel.CommunityName = communityModel.FriendlyName;

            communityViewModel.BannerImageLocation = communityModel.BannerLocation;

            communityViewModel.Header = "Welcome to the Ruddington Community Response Team HelpMyStreet page";

            communityViewModel.HeaderHTML = @"
                    <p class='mt-sm mb-xs'>
                        	In our community there’s always somebody here to help, there’s no need for anyone to struggle alone.
                            We’re the Ruddington Community Response Team, here to help with:
                    </p>
                    <ul class='tick-list mt-xs mb-sm compact-list'>
                        <li>Shopping for essentials</li>
                        <li>Collecting prescriptions</li>
                        <li>A friendly chat</li>
                        <li>Local errands</li>
                        <li>Anything else, just ask!</li>
                    </ul>
                    ";


            communityViewModel.HeaderVolunteerButtonText = "";
            communityViewModel.HeaderHelpButtonText = "";

            communityViewModel.CommunityVolunteersHeader = "Welcome to the Ruddington Community Response Team HelpMyStreet page";
            communityViewModel.CommunityVolunteersTextReadMore = false;
            communityViewModel.CommunityVolunteersTextHtml =
                 @"<p>Supported by the Parish Council and the Ruddington Village Centre Partnership (RVCP), we’re a group of local volunteers set up to provide a good neighbour network for those who need a little bit of extra help.</p>
                    <p>If you’d like some local volunteer help just ask by clicking on one of the ‘Request Help’ buttons on this page or text ‘Help’ for free to 60002. You can also give the Parish Council a call on 0115 914 6660 (usual office hours Monday to Friday 9.30am to 12.30pm). Our volunteers are local people supporting our wonderful village.</p>
                    <p>To join us sign up above or to get in touch, email <a href='mailto:ruddington@helpmystreet.org'>ruddington@helpmystreet.org</a></p> 
                    <p>With thanks to Peter McConnochie of <a href='https://www.urbanscot.co.uk' target='_blank'>urbanscot.co.uk</a> for supplying the majority of the wonderful photographs of our village and volunteers.</p> 
                    <p>* RVCP is a collaboration of local business owners, Ruddington Parish Councillors and residents; volunteering together to maintain a vibrant village centre, bring people together and develop opportunities for village residents.</p>
                    ";

            communityViewModel.RequestHelpHeading = @"How can we help?";

            communityViewModel.RequestHelpText = @"We’ve got shoppers, sewers and hot-meal makers; walkers, talkers and home-work helpers all ready and waiting to help you!";

            communityViewModel.ProvideHelpHeading = "Volunteer with us!";

            communityViewModel.ProvideHelpText_NotGroupMember = "Join us to help your neighbours. Just let us know when, where and how you can help. You can choose to help a little, or to help a lot! We're grateful for every contribution.";
            communityViewModel.ProvideHelpText_GroupMember = "Thanks for being part of the Ruddington Community Response Team. Click below to view help requests in your area.";

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Ruddington Parish Council",
                    Role = "Proudly supported by the Parish Council",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/RPC-logo.jpeg"
                },
                new CommunityVolunteer()
                {
                    Name = "Ruddington Village Centre Partnership",
                    Role = "Proudly supported by RVCP*",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/RVCP.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Han, Mark & Stella",
                    Role = "Market Volunteers",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/HanMarkStella.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Leia",
                    Role = "Market Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/Leia.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Alex",
                    Role = "Community volunteer",
                    Location = "(photograph supplied by Pam Pearce)",
                    ImageLocation = "/img/community/ruddington/Alex.png"
                },
                new CommunityVolunteer()
                {
                    Name = "Andrew",
                    Role = "Market Volunteer",
                    Location = "",
                    ImageLocation = "/img/community/ruddington/Andrew.jpg"
                },
            };


            communityViewModel.UsefulLinksHtml = @"<p><a href='https://ruddingtonparishcouncil.gov.uk'>Ruddington Parish Council</a></p>
                                                   <p><a href='https://www.facebook.com/groups/892154851236247'>Ruddington COVID-19 Mutual Aid</a> (Facebook group)</p>
                                                   <p><a href='http://ruddington.info'>Ruddington.info</a></p>";


            communityViewModel.ShowRequestHelpPopup = true;
            communityViewModel.RequestHelpPopupText = "<p>Just to confirm, is the help needed in <b>Ruddington</b>?</p>";
            communityViewModel.RequestHelpPopupRejectButtonText = "No, somewhere else";
            communityViewModel.RequestHelpPopup2Text = @"<p>The <b>Ruddington Community Response Team</b> offer help in <b>Ruddington</b>. But don’t worry, HelpMyStreet has volunteers all over the UK.</p>
                                                            <p>Request help from someone near you by clicking below.</p>";
            communityViewModel.RequestHelpPopup2Destination = $"/request-help/{Base64Utils.Base64Encode((int)Groups.Generic)}/{communityViewModel.EncodedGroupId}";

            communityViewModel.AllowJoinOurGroup = true;
            communityViewModel.JoinOurGroupButtonText = "Join Our Group";
            communityViewModel.JoinGroupPopupText = "<p>Would you like to join the <b>Ruddington Community Response Team</b>?</p>";

            communityViewModel.AllowLeaveOurGroup = true;
            communityViewModel.LeaveGroupPopupText = "<p>Are you sure you want to leave the <b>Ruddington Community Response Team</b>?</p>";

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetAgeUKLSL(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();
            CommunityModel communityModel = await GetCommunityDetailByKey("ageuklsl");

            int groupId = await _groupService.GetGroupIdByKey("ageuklsl", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.HomeFolder = "ageUK";
            communityViewModel.Latitude = 52.95;
            communityViewModel.Longitude = -0.2;
            communityViewModel.ZoomLevel = communityModel.ZoomLevel;

            communityViewModel.showFeedbackType = Models.Feedback.FeedbackMessageType.Group;
            communityViewModel.groupKey = "ageuklsl";
            communityViewModel.CommunityName = communityModel.FriendlyName;
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
                        If you can help us deliver wellbeing packs to veterans in your area please sign up below - and click to view open requests in your area.
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

            communityViewModel.HelpExampleCards = new Models.HelpExampleCardsViewModel()
            {
                Example1 = "Deliver a wellbeing parcel to a veteran in Grantham",
                Example2 = "Collect a prescription for an older lady in Lincoln",
                Example3 = "Post a letter for a gentleman in Spalding"
            };

            communityViewModel.AllowJoinOurGroup = true;
            communityViewModel.JoinOurGroupButtonText = "Join Our Group";
            communityViewModel.JoinGroupPopupText = "<p>Would you like to join the <b>Age UK Lincoln and South Lincolnshire</b> team?</p>";

            communityViewModel.AllowLeaveOurGroup = true;
            communityViewModel.LeaveGroupPopupText = "<p>Are you sure you want to leave the <b>Age UK Lincoln and South Lincolnshire</b> team?</p>";

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetAgeUKWirral(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();

            int groupId = await _groupService.GetGroupIdByKey("ageukwirral", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
            communityViewModel.HomeFolder = "ageUK/wirral";
            communityViewModel.Latitude = 53.397320;
            communityViewModel.Longitude = -3.042670;
            communityViewModel.ZoomLevel = 11;

            communityViewModel.showFeedbackType = Models.Feedback.FeedbackMessageType.Group;
            communityViewModel.groupKey = "ageukwirral";

            communityViewModel.CommunityName = "Age UK Wirral";
            communityViewModel.CommunityShortName = "Age UK Wirral";

            communityViewModel.BannerImageLocation = "/img/community/ageUK/wirral/age-uk-wirral-banner.png";

            communityViewModel.Header = "In the Wirral, help is always available!";
            communityViewModel.HeaderHTML = @"
                    <p class='mt-sm mb-xs'>
                        Age UK Wirral are proud to have an amazing range of services for older people in our local communities. Our services are supported by volunteers who are here to help with:
                    </p>
                    <p class='mt-sm mb-xs'>
                        <ul class='tick-list mt-xs mb-sm compact-list'>
                            <li>Shopping for essentials</li>
                            <li>Collecting prescriptions</li>
                            <li>Staying warm and healthy in the chilly winter</li>
                            <li>Door-to-door transport for medical appointments</li>
                        </ul>
                    </p>";
            communityViewModel.CommunityVolunteersHeader = "Welcome from Age UK Wirral";
            communityViewModel.HeaderVolunteerButtonText = null;
            communityViewModel.HeaderButtonWidth = 12;

            communityViewModel.CommunityVolunteersTextHtml =
                 @"<p>
                    Age UK Wirral is an independent, local charity and we have been working in the local community to help 
                    older people, their families and carers for over 70 years. We have 700 dedicated staff and volunteers helping 
                    us to deliver services and activities for older people in the Wirral.
                </p>
                <p>
                     In March 2020 we launched our Covid-19 Emergency Response Services to support people with shopping, 
                     prescription collections and wellbeing support. In six months we have carried out over 3,000 shopping trips, 
                     collected over 500 prescriptions, lent 250 people tablets and data sim cards and made more than 35,000 wellbeing calls 
                     to isolated people in the community.  These services will continue to support the community and are vital to keep people safe, well and connected.
                </p>
                <p>
                     The services on this page are a selection of those available, focussing on areas where we need ad hoc 
                     volunteer assistance.
                </p>
                <p>
                    To find out more about our other services and the support we can provide, please go to our main website <a href=""www.ageukwirral.org.uk"">www.ageukwirral.org.uk</a>
                    or call us on 0151 482 3456.
                </p> 
                ";
            communityViewModel.ShowRequestHelp = false;

            communityViewModel.RequestHelpHeading = @"How can we help?";

            communityViewModel.RequestHelpText = @"We support older people, their families and carers. To find out more about our other services and the support we can provide, please go to our website or call us on 0151 482 3456.";
            communityViewModel.HeaderVolunteerButtonText = "Age UK Wirral relies on volunteers to help us offer vital services in the local community. Would you like to lend a hand?";

            communityViewModel.AllowJoinOurGroup = true;
            communityViewModel.JoinOurGroupButtonText = "Join Our Group";
            communityViewModel.JoinGroupPopupText = "<p>Would you like to join <b>AgeUK Wirral</b>?</p>";

            communityViewModel.AllowLeaveOurGroup = true;
            communityViewModel.LeaveGroupPopupText = "<p>Are you sure you want to leave <b>AgeUK Wirral</b>?</p>";

            communityViewModel.ProvideHelpHeading = "Volunteer with us!";

            communityViewModel.ProvideHelpText_NotGroupMember = "Join us to help your neighbours. Just let us know when, where and how you can help. You can choose to help a little, or to help a lot! We’re grateful for every contribution.";
            communityViewModel.ProvideHelpText_GroupMember = "Thanks for being part of AgeUK Wirral.  Click below to view help requests in your area.";

            communityViewModel.ShowVisitWebsite = true;
            communityViewModel.VisitWebsiteHeading = "How can we help?";
            communityViewModel.VisitWebsiteText = "We support older people, their families and carers. To find out more about our other services and the support we can provide, please go to our website or call us on 0151 482 3456.";
            communityViewModel.VisitWebsiteButtonText = "Go to our website";
            communityViewModel.WebsiteUrl = "https://www.ageuk.org.uk/wirral/";
            communityViewModel.HeaderVisitWebsiteButtonText = "To ask for help, please go to our website or call us on 0151 482 3456.";

            communityViewModel.CommunityVolunteers = new List<CommunityVolunteer>()
            {
                new CommunityVolunteer()
                {
                    Name = "Karen Giner",
                    Role = "Home & Communities Volunteer Coordinator",
                    ImageLocation = "/img/community/ageuk/wirral/KG.jpg"
                },
                new CommunityVolunteer()
                {
                    Name = "Catherine Sindall",
                    Role = "Administrator",
                    ImageLocation = "/img/community/ageuk/wirral/CS.jpg"
                }
            };

            communityViewModel.UsefulLinksHtml =
                @"<p><a href=""https://www.ageuk.org.uk/wirral/"">Age UK Wirral Website</a> - Find out about our extensive range of services, donations, charity shops and further details about our organisation.</p>
                <p><a href=""https://www.facebook.com/ageukwirral/"">Age UK Wirral Facebook</a> - Facebook page of Age UK Wirral.</p>
                <p><a href=""https://www.wirralinfobank.co.uk/"">Wirral InfoBank</a> - The place where Wirral residents can find local community support services, online events and up-to-date advice and information about coronavirus (COVID-19).</p>
                <p><a href=""/pdf/ageUK/wirral/WirralVolunteerInstructions.pdf"">Volunteer Instructions</a> - Read our how-to guide (including frequently asked questions).</p>";

            communityViewModel.ShowHelpExampleCards = false;

            return communityViewModel;
        }

        private async Task<CommunityViewModel> GetFtLOS(CancellationToken cancellationToken)
        {
            CommunityViewModel communityViewModel = new CommunityViewModel();
            communityViewModel.showFeedback = true;
            communityViewModel.ShowHelpExampleCards = false;
            communityViewModel.showFeedbackType = Models.Feedback.FeedbackMessageType.FaceCovering;

            int groupId = await _groupService.GetGroupIdByKey("ftlos", cancellationToken);
            communityViewModel.EncodedGroupId = Base64Utils.Base64Encode(groupId);
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
                    <p>If you’re a FTLOS sewer and need more materials contact your local group administrator or email <a href='mailto:requestmaterials.ftlos@outlook.com'>requestmaterials.ftlos@outlook.com</a></p>
            ";

            communityViewModel.RequestHelpHeading = "Do you need a face covering?";
            communityViewModel.RequestHelpText = "If you’d like some For the Love of Scrubs face coverings for yourself, your family or an organisation, request them now. We'll do our best to help! ";
            communityViewModel.RequestHelpButtonText = "Request Face Coverings";

            communityViewModel.ProvideHelpHeading = "Volunteer with us!";
            communityViewModel.ProvideHelpText_NotGroupMember = "If you’d like to join For the Love of Scrubs (or register as an existing member) sign up now. We’ll send you everything you need to get started (except for the sewing machine!)";
            communityViewModel.ProvideHelpText_GroupMember = "Thanks for being part of For the Love of Scrubs.  Click below to view help requests.";
            communityViewModel.ProvideHelpButtonText_LoggedOut = "Sew with FTLOS";

            communityViewModel.HelpExampleCards = new Models.HelpExampleCardsViewModel()
            {
                Example1 = "6 plain face coverings to safely commute",
                Example2 = "Bright face coverings to cheer my mum up in hospital",
                Example3 = "10 donated face coverings for care home visitors"
            };

            communityViewModel.AllowJoinOurGroup = true;
            communityViewModel.JoinOurGroupButtonText = "Join Our Group";
            communityViewModel.JoinGroupPopupText = "<p>Would you like to join <b>For the Love of Scrubs</b>?</p>";

            communityViewModel.AllowLeaveOurGroup = true;
            communityViewModel.LeaveGroupPopupText = "<p>Are you sure you want to leave <b>For the Love of Scrubs</b>?</p>";

            return communityViewModel;
        }
    }
}
