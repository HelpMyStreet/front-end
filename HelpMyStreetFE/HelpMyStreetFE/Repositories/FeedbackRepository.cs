using HelpMyStreet.Contracts.FeedbackService.Request;
using HelpMyStreet.Contracts.FeedbackService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Feedback;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
namespace HelpMyStreetFE.Repositories
{
    public class FeedbackRepository : BaseHttpRepository, IFeedbackRepository
    {
        public FeedbackRepository(
            IConfiguration configuration,
            ILogger<FeedbackRepository> logger,
            HttpClient client) : base(client, configuration, logger, "Services:Feedback")
        {
        }

        public async Task<bool> GetFeedbackExists(int JobId, RequestRoles requestRole, int? userId)
        {
            GetFeedbackExistsRequest request = new GetFeedbackExistsRequest()
            {
                JobId = JobId,
                RequestRoleType = new RequestRoleType() { RequestRole = requestRole },
                UserId = userId
            };
            string json = JsonConvert.SerializeObject(request);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/GetFeedbackExists", data);
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<bool, FeedbackServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content;
            }
            return false;
        }

        public async Task<List<Testimonial>> GetTestimonials()
        {
            var messageList = new List<Testimonial>()
            {
                new Testimonial()
                {
                    Tagline = "Huge thank-you",
                    Message = "All of the staff here at the Nottinghamshire YMCA would like to say a huge thank you for the homemade face coverings provided by volunteers from HelpMyStreet. They're well-made and all different which makes them much more interesting than normal disposable masks! We're a charity and requested 100 face coverings as a donation to help our staff and residents stay safe during this pandemic.",
                    Person = "Gillian from Nottingham asked for help",
                    Type = FeedbackMessageType.FaceCovering,
                    B2BFeedback = true,
                },
                new Testimonial()
                {
                    Tagline = "Lovely chat",
                    Message = "I volunteered to help out with the Age UK Vitals for Veterans campaign. Delivering the wellbeing package was very easy, I collected it from my local Age UK branch and was told where to drop it off. I had a lovely chat with the recipient of the parcel, an RAF veteran who was out tending to his garden when I arrived. I'm looking forward to the next delivery!",
                    Person = "Sean in Boston provided some help",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuklsl"
                },
                new Testimonial()
                {
                    Tagline = "Grateful thanks",
                    Message = "The masks have arrived and are really lovely, very comfortable and of very good quality. Please pass on our grateful thanks to all that took the time to sew these masks. Especially to the wonderful Gill who very kindly made 60 of the masks!",
                    Person = "Ann-Marie from Boston asked for help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new Testimonial()
                {
                    Tagline = "Excellent service",
                    Message = "Just want to say as soon as I requested the masks they were in process to be made. Delivered within 2 days and excellent quality for adults and kids. Thank you so much at a difficult financial time I managed to purchase for the whole family too. Excellent service.",
                    Person = "Gurmeet from Leicester asked for help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new Testimonial()
                {
                    Tagline = "Really great",
                    Message = "The process of requesting face coverings through HelpMyStreet couldn’t have been easier. I requested four face coverings for my children and mentioned the mixture of boys and girls. Within a week 4 really stylish face coverings had been posted to our home and the children were thrilled with the designs (Harry Potter etc). I wouldn’t hesitate to recommend HelpMyStreet or Collette who made the face coverings. Not only were the face coverings really great but it also feels reassuring that our donation will cover Collette’s cost but also go to charity.",
                    Person = "Mark from Coventry asked for help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new Testimonial()
                {
                    Tagline = "A real boost!",
                    Message = "I was only able to make a few face masks, but it was good to know that they were going to people who needed them. It showed me that you don't need to do a huge amount of work to really help others. I had a lovely thank you message from one recipient and that was a real boost!",
                    Person = "Sue in Chesterfield provided some help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new Testimonial()
                {
                    Tagline = "Totally happy",
                    Message = "I was extremely grateful and totally happy with the masks that were made for myself, my granddaughter and my son. They fitted us perfectly and most importantly my 9 year old granddaughter was comfortable, proud & happy wearing hers for a long trip to France on Eurostar. She got to choose the material pattern thanks to Kiyomi’s communication with me. Praise to all 😘",
                    Person = "Sue from Hornchurch asked for help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new Testimonial()
                {
                    Tagline = "Delighted",
                    Message = "I am delighted with my face coverings, they are perfect and they fit both my husband and I perfectly. Thank you so much to the lovely person who made them.  They've washed well too.",
                    Person = "Sue from Llandudno asked for help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new Testimonial()
                {
                    Tagline = "Can't recommend it enough",
                    Message = "HelpMyStreet has allowed us to keep what was great about our COVID-19 response, but set ourselves up to keep this going even when things hopefully get back to being a bit more normal. We can’t recommend it highly enough.",
                    Person = "Dave is a Community Organiser in Tankersley",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "tankersley",
                    B2BFeedback = true,
                },
                new Testimonial()
                {
                    Tagline = "Absolute community spirit",
                    Message = "Big 'Thank you from the Barkantine Practice', was lovely to receive these masks today and give them to the staff. So good to see a range of colours and sizes and materials and also catering for the boys, we are truly grateful.  Thanks for all the work you are doing, absolute community spirit at its best.",
                    Person = "Sasha is a GP in Tower Hamlets who requested face coverings to help staff commute on public transport",
                    Type = FeedbackMessageType.FaceCovering,
                    B2BFeedback = true,
                },
                new Testimonial()
                {
                    Tagline = "Local people",
                    Message = "The ability to shape the content and presentation of local material - on the website and with the posters and flyers - makes it much more likely that local people will access and use this service",
                    Person = "Kate organises a local group in South Yorkshire who built a community page on HelpMyStreet",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "tankersley",
                    B2BFeedback = true,
                },
                new Testimonial()
                {
                    Tagline = "A privilege",
                    Message = "It has been a privilege to join HelpMyStreet and to deliver the wellbeing packages for Age UK. To be able to give something back to others who have given to us in the past is most rewarding.",
                    Person = "Sharon in Stamford provided some help",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuklsl"
                },
                new Testimonial()
                {
                    Tagline = "Very rewarding",
                    Message = "I am enjoying being a volunteer for the Age UK Vitals for Veterans campaign. Helping those who have served their country is very rewarding and I am only too pleased to be able to help.",
                    Person = "Steve in Spalding provided some help",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuklsl"
                },
                new Testimonial()
                {
                    Tagline = "A fantastic experience",
                    Message = "Working with the team from Factor 50 on the HelpMyStreet initiative has been a fantastic experience.  We have developed a wonderful partnership which has genuinely helped us to reach so many more older people and willing volunteers alike for our Vitals for Veterans project.  They are so flexible and engaged with us that we are now also looking at other ways that we can work with them on different services and projects we have.",
                    Person = "Michele Jolly, CEO at Age UK LSL",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuklsl",
                    B2BFeedback = true,
                },
                new Testimonial()
                {
                    Tagline = "A valuable tool",
                    Message = "Using HelpMyStreet enables us to recruit volunteers and support older people in communities that are sometimes harder to reach. It’s a valuable tool in a rural county.",
                    Person = "Nicki Lee, Senior Volunteer Coordinator at Age UK LSL",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuklsl",
                    B2BFeedback = true,
                },
                new Testimonial()
                {
                    Tagline = "A wonderful village",
                    Message = "Thank you from the bottom of my heart to you and the team for helping my parents through this difficult time. It's so appreciated. What you guys did and will continue to do was/is amazing. Ruddington is a wonderful village ❤",
                    Person = "Leigh requested help for his parents in Ruddington",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ruddington",
                },
                new Testimonial()
                {
                    Tagline = "A caring bunch",
                    Message = "I am proud and pleased to help with the Covid-19 group in my village. There was an obvious need for our services and we came together as a community in a brilliant way. Lots of people volunteered to assist without hesitation which just shows what a caring bunch we are.",
                    Person = "Pam from Ruddington provided help",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ruddington",
                    B2BFeedback = true
                },
                new Testimonial()
                {
                    Tagline = "Really keen to get involved",
                    Message = "When I heard that Ruddington was in need of volunteers to assist with the Covid response I was really keen to get involved. I have children at both local schools and it was great to be able to deliver schoolwork out to their classmates who were stuck at home. Although they knew what it meant when they saw me walking towards the door, most of the kids were excited to be getting a delivery from school and to see what fresh challenges had been set by their teachers.",
                    Person = "Harvey from Ruddington provided help",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ruddington",
                    B2BFeedback = true
                },
                new Testimonial()
                {
                    Tagline = "A poem of thanks!",
                    Message = "Thank you to our local Pub, Thank you for all for our delicious grub, Thank you to everyone involved in the Ruddington Covid 19 Mutual Aid Group, Thank you also for keeping us all in the Loop, For arranging this, Fabulous dish xx",
                    Person = "Pat and George wanted to say thanks",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ruddington",
                },

                new Testimonial()
                {
                    Tagline = "Keeping us alive",
                    Message = "These services are keeping us alive - we very grateful for the kindness and support of Age UK Wirral.",
                    Person = "Mr and Mrs M",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageukwirral",
                },
                new Testimonial()
                {
                    Tagline = "People who care",
                    Message = "Being in touch with Age UK Wirral has made me realise that there are people who care. The volunteers who have done my shops and prescription collections have been lovely people - they are the unsung heroes.",
                    Person = "Lesley Fulton",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageukwirral",
                },
                new Testimonial()
                {
                    Tagline = "An absolute pleasure",
                    Message = "I've been volunteering since April, doing shopping and prescription collections. It's been an absolute pleasure supporting our community and receiving the phone calls from the staff every week  - it's just wonderful. Thank you very much and keep up the good work.",
                    Person = "Natalie, volunteer",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageukwirral",
                },
                new Testimonial()
                {
                    Tagline = "Making a difference",
                    Message = "I'd like to think that someone would be there to help me if I had a health condition. I've got spare time and I'm just pleased to be putting it to good use. It's lovely to know I am making a difference.",
                    Person = "Tricia, volunteer",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageukwirral",
                },
                new Testimonial()
                {
                    Tagline = "What more could you ask for",
                    Message = "I like the company, the meals are very good, the carers are good, we've got bingo, quizzes, netball, chiropody and loads of activities they do to keep us happy. You get picked up, dropped off... what more could you ask for?",
                    Person = "Alice",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-southkentcoast",
                },                
                new Testimonial()
                {
                    Tagline = "Always friendly",
                    Message = "I have been here three times a week for the past 4 years. I find the staff and volunteers are always friendly and very helpful. Age UK is a good meeting place to make friends and I enjoy coming here.",
                    Person = "Bill",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-southkentcoast",
                },
                new Testimonial()
                {
                    Tagline = "I couldn't do without it",
                    Message = "I have been coming twenty years now and couldn't do without it. The food is very good, the people are very nice and the staff are wonderful and do things to entertain us.",
                    Person = "Annie",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-southkentcoast",
                },
                new Testimonial()
                {
                    Tagline = "Everyone's so caring",
                    Message = "I love coming for a good laugh. I like the people and the carers, I've made some good friends. Everyone's so caring here.",
                    Person = "Keith",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-southkentcoast",
                },
                new Testimonial()
                {
                    Tagline = "I couldn't do without it",
                    Message = "It gives me something to look forward to each week, I don’t know anyone in this area, and although people chat to me when I’m out with the dog, I don’t have any friends here, so can go days without really seeing anyone. It’s reassuring knowing I have someone just down the road if anything happens.",
                    Person = "Yvonne",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-favershamandsittingbourne",
                },
                new Testimonial()
                {
                    Tagline = "Much needed social interaction",
                    Message = "I have been volunteering for Age UK as a befriender since January this year. Befriending for me gives me a break from the computer, and some much needed social interaction in the week, as well as the feeling like I am doing something worthwhile for someone else at the same time. No matter what age you are, I feel if you sit around the house with no one to talk to for days on end, it is very easy to suddenly feel isolated and dispirited. I feel I benefit from the social side of volunteering, just as much as Yvonne does.",
                    Person = "Robyn",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-favershamandsittingbourne",
                },
                new Testimonial()
                {
                    Tagline = "Good friends",
                    Message = "Les and I get on like a house on fire and it is one of the most nourishing relationships I could have. He's an ex-miner and has just tuned 91 though you'd never know it. He has a cracking sense of humour and we've become really good friends over the last year and I wouldn't miss our weekly meet up for the world and neither would Les. We talk about all manner of things and it truly is a two way process of learning and growing in openness and understanding.",
                    Person = "Simon",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-favershamandsittingbourne",
                },
                new Testimonial()
                {
                    Tagline = "Mary Is Amazing",
                    Message = "Ordinarily a volunteer for Age UK, but when Lincolnshire’s NHS teamed up with our voluntary partners to deliver the Covid-19 vaccination programme, Mary started helping in our vaccination centres (she’s even roped her husband into car parking duties!). Mary is amazing! 👏 Thank you to all our volunteers for helping bring the Covid - 19 vaccination to the people of Lincolnshire.'",
                    Person = "NHS Lincolnshire Clinical Commissioning Group",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "lincs-volunteers",
                },
                new Testimonial()
                {
                    Tagline = "Craig, we salute you!",
                    Message = "A former RAF pilot and currently furloughed from his job as an airline pilot, Craig’s volunteering role is to make it as easy as possible for the clinical staff to do their jobs. Along with all the other volunteers Craig is inspired to see the older generation lead from the front receiving their vaccines.",
                    Person = "NHS Lincolnshire Clinical Commissioning Group",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "lincs-volunteers",
                },
                new Testimonial()
                {
                    Tagline = "What a team!",
                    Message = "Usually a GP practice nurse, Emily has been spending lots of her time helping us set up another of Lincolnshire’s local vaccination sites. Her Dad, a retired GP is also part of the national programme.",
                    Person = "NHS Lincolnshire Clinical Commissioning Group",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "lincs-volunteers",
                },
                new Testimonial()
                {
                    Tagline = "Thank you Dr Thompson!",
                    Message = "Also known as James, Dr Thompson is one of the county’s GPs delivering the local vaccination site programme, whilst continuing to keep other services running back at the practice.",
                    Person = "NHS Lincolnshire Clinical Commissioning Group",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "lincs-volunteers",
                },
                new Testimonial()
                {
                    Tagline = "Making sure people feel safe and well",
                    Message = "A retired nurse, Chris is eagerly awaiting her vaccinator status to come through and until then is volunteering in the centre’s Observation Area, making sure people feel safe and well immediately after their jab.",
                    Person = "NHS Lincolnshire Clinical Commissioning Group",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "lincs-volunteers",
                },
                new Testimonial()
                {
                    Tagline = "At the forefront",
                    Message = "One of our primary care network leads, Amanda has been at the forefront of organising another of Lincolnshire’s local vaccination sites. From staff, to signs, to cleaning, she’s thought of it all.",
                    Person = "NHS Lincolnshire Clinical Commissioning Group",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "lincs-volunteers",
                },
                new Testimonial()
                {
                    Tagline = "Brightens up her week",
                    Message = "Liz takes herself on her usual dog walk with Oscar the gorgeous Golden Retriever and walks to her client’s home where they have a socially distanced chat through the window. According to the client – the regular visits from Liz and Oscar really brighten her week!",
                    Person = "A very grateful befriending client",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageconnects-cardiff",
                },
                new Testimonial()
                {
                    Tagline = "Something special",
                    Message = "One of the reasons it's so rewarding to help your community is that the results are so visible and significant. There's something special about knowing that your time and donation has made a difference that you can actually see.",
                    Person = "Jeff Hawkins, CEO Age Connects Cardiff and the Vale",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageconnects-cardiff",
                },
                new Testimonial()
                {
                    Tagline = "Absolute lifeline",
                    Message = "Thank you – to Age Connects and, more specifically, to Jules for being an absolute lifeline to Ronnie over the past few weeks. I honestly dread to think what the outcome would have been if you had not come to the rescue. Jules has gone the extra mile and assisted with things such as taking out his rubbish as well as ensuring that he has all the basics to keep him going. She has been invaluable.",
                    Person = "Sara, the  niece of an 80-year-old Cardiff client",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageconnects-cardiff",
                },
                new Testimonial()
                {
                    Tagline = "Thank you",
                    Message = "I am touched and moved by this generosity. God bless the hands of those who have prepared the food boxes, provided the contents and organised and administered the distribution. Thank you.",
                    Person = "Shared on Facebook (Caroline Beaumont)",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "meadows-community-helpers",
                },
                new Testimonial()
                {
                    Tagline = "Wasim, you are a star",
                    Message = "A big thankyou to Wasim Amin  one of our new Trustees who is a local taxi driver and a community activist for delivering us door to door to the vaccination Centre at Kings Meadow Lenton Lane for our first jab . Thanking him for his patience and help. Whilst we waited in line and went through the process. It is a well organised system doing checks all the way through. The staff are wonderful and helpful.  Thank you once more  Wasim you are a star.",
                    Person = "Shared by The Bridges Community Trust",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "meadows-community-helpers",
                },
                new Testimonial()
                {
                    Tagline = "An absolute lifeline",
                    Message = "My husband and I would like to thank you all so much for all the help you arranged for vulnerable people during the lockdown. We had to make use of the service provided for medication several times and it was an absolute lifeline. We are a long way away from our family and at times things were quite difficult, but it felt so good to know that there was a backup there if we needed it. Thank you very much.",
                    Person = "Local residents",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "southwell",
                },
                new Testimonial()
                {
                    Tagline = "Many thanks",
                    Message = "Many thanks Torpedos for your support, especially Sarah whose smile and cheery greeting was so welcome at my lowest ebb.",
                    Person = "Margaret, local resident",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "southwell",
                },
                new Testimonial()
                {
                    Tagline = "",
                    Message = "I love coming to Positive Living… it stops me from being lazy!",
                    Person = "Ada, a Positive Living Day Care regular",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-midmersey",
                },
                new Testimonial()
                {
                    Tagline = "",
                    Message = "I can’t thank Age UK Mid Mersey enough for matching me with a volunteer. It’s good to know people care.",
                    Person = "Mrs W, a user of our befriending service",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-midmersey",
                },
                new Testimonial()
                {
                    Tagline = "",
                    Message = "When my volunteer visits, it gives me a reason to get out of bed and sit up, I enjoy our chats.",
                    Person = "Mrs F, a user of our befriending service",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-midmersey",
                },
                new Testimonial()
                {
                    Tagline = "",
                    Message = "I look forward to my weekly visits. It gives my life a purpose now and stops me getting bored.",
                    Person = "Mrs T, a user of our befriending service",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-midmersey",
                },
                new Testimonial()
                {
                    Tagline = "",
                    Message = "Me and my volunteer get on great. It’s so good to talk to someone who gives back a good conversation.",
                    Person = "Mr B, a user of our befriending service",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-midmersey",
                },
                new Testimonial()
                {
                    Tagline = "Great service I can rely on",
                    Message = "Paul, the volunteer from Age UK Mid Mersey, is great. I trust him to support with the little jobs I can't get to. He's done a few jobs for me this year helping to put up shelves, pictures and my curtain pole. Thanks Age UK Mid Mersey - A great service I can always rely on.",
                    Person = "Mrs R, a happy customer of our Helping Hands service",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuk-midmersey",
                },

                new Testimonial()
                {
                    Tagline = "New friendships",
                    Message = "It’s wonderful to hear from local people who have already made new connections and friendships thanks to the Good Neighbour Schemes. We are proud to be funding Community Lincs so it can continue to build on this success, helping more people tap into this support and feel less isolated. Thanks to National Lottery players, these Good Neighbour Schemes will bring communities together, empowering them to thrive.",
                    Person = "Matt Poole, Senior Head of Funding for the Midlands at The National Lottery Community Fund",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "English"
                },

                new Testimonial()
                {
                    Tagline = "One of the best things I have done",
                    Message = "GNS is one of the best things I have done in my life, despite taking considerable effort and time to organise and manage. Without Community Lincs our Good Neighbour Scheme would not have been organised. Neither would it have been so confident and competent. Community Lincs are our people to go to for support and information; we would feel lost without them.",
                    Person = "Good Neighbour Scheme Volunteer",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "English"
                },

                new Testimonial()
                {
                    Tagline = "People who really care",
                    Message = "I couldn’t manage without my Good Neighbours. I have used the service mainly for transport, but the support and friendship is fantastic. It makes a huge difference knowing there are people there who really care.",
                    Person = "Good Neighbour Scheme Service User",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                },

                new Testimonial()
                {
                    Tagline = "Best thing to happen in this area for many a year",
                    Message = "Our Good Neighbour Scheme is the best thing to happen in this area for many a year. It gives me peace of mind that there is always someone to help with my many regular trips to various clinics, and I really enjoy the regular visits from my befriender. Little things such as making telephone calls on my behalf make such a difference.",
                    Person = "GNS Service User",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "English"
                },

                new Testimonial()
                {
                    Tagline = "Very rewarding",
                    Message = "The benefits for myself have been very rewarding - meeting new people and learning new skills.",
                    Person = "GNS Volunteer",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "English"
                },

                new Testimonial()
                {
                    Tagline = "Coffee mornings",
                    Message = "I really like attending the good neighbour coffee mornings; I have met lots of new people and made friends.",
                    Person = "GNS Service User",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "English"
                },

                new Testimonial()
                {
                    Tagline = "Impressive",
                    Message = "Community Lincs provided the spark that ignited interest in setting-up our Good Neighbour Scheme and has provided ongoing support from initial consultation with the local community through presentations, networking with existing organisations and co-ordinating a community survey to the formation of a constituted group with policies, procedures and a bespoke IT system culminating in a fully operational scheme. The breadth of support provided has been particularly impressive.",
                    Person = "GNS Volunteer",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "English"
                },

                new Testimonial()
                {
                    Tagline = "My days are much brighter now",
                    Message = "My days are much brighter now that my GNS volunteer visits me, as I used to be quite lonely. They also help me get to my hospital appointments, as my family live a long way away.",
                    Person = "GNS Service User",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "English"
                },

                new Testimonial()
                {
                    Tagline = "Сега дните ми са много по-светли",
                    Message = "Дните ми са много по-светли сега, когато ме посещава моят доброволец от СД, докато преди бях доста самотна/самотен. Те  ми помагат да отида до болницата за преглед, защото семейството ми живее далеч.",
                    Person = "Потребител на услугата СД",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Bulgarian"
                },

                new Testimonial()
                {
                    Tagline = "Хора, които наистина ги е грижа",
                    Message = "Не бих могъл/могла да се справя без моите добри съседи. Използвал/а съм услугата главно за транспорт, но подкрепата и приятелството са невероятни. От огромно значение е да знаеш, че има хора, които наистина ги е грижа.",
                    Person = "Потребител на услугата „Програма за Добросъседство“",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Bulgarian"
                },

                new Testimonial()
                {
                    Tagline = "Сутрешно кафе",
                    Message = "Много ми харесва да участвам в добросъседските срещи за сутрешно кафе; срещнах много нови хора и намерих приятели.",
                    Person = "Потребител на услугата СД",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Bulgarian"
                },

                new Testimonial()
                {
                    Tagline = "Най-хубавото, което се е случвало в този район от много години насам",
                    Message = "Нашата програма  за добросъседство е най- хубавото нещо, което се е случвало в този район от много години насам. Тя ми дава спокойствие, като знам, че  винаги има кой да ми помогне при честите ми посещения в различни клиники, и наистина се радвам на редовните посещения на моя приятел. Малки  неща, като например провеждането на телефонни разговори от мое име, са от огромно значение.",
                    Person = "Потребител на услугата СД",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Bulgarian"
                },
                new Testimonial()
                {
                    Tagline = "Едно от най-хубавите неща, които съм правил/а",
                    Message = "СД е едно от най-хубавите неща, които съм правил/а в живота си, въпреки че организацията и управлението изискват значителни усилия и време. Без „Къмюнити Линкс“ нашата програма  за добросъседство нямаше да се реализира. Нито пък щеше да е толкова надеждна и компетентна. „Къмюнити Линкс“ са хората, към които се обръщаме за подкрепа и информация; без тях бихме били като изгубени.",
                    Person = "Доброволец по Програмата  за Добросъседство",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Bulgarian"
                },
                new Testimonial()
                {
                    Tagline = "Нови приятелства",
                    Message = "Чудесно е да чуваме мнението на местни хора, които вече са създали нови връзки и приятелства благодарение на програмите  за добросъседство. Горди сме, че финансираме „Къмюнити Линкс“, за да може тя да продължи да надгражда този успех, като помага на все повече хора да се възползват от тази подкрепа и да се чувстват по-малко изолирани. Благодарение на участниците в Националната лотария, програмите  за добросъседство ще обединяват общностите и ще им дадат възможност да се развиват.",
                    Person = "Мат Пул, старши ръководител „Финансиране за Мидлъндс“ към Обществения Фонд на Националната лотария",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Bulgarian"
                },
                new Testimonial()
                {
                    Tagline = "Впечатляващо",
                    Message = "„Къмюнити Линкс“ даде искрата, която запали интереса към създаването на нашата програма  за добросъседство. Те осигуряват непрестанна подкрепа – от първоначалните консултации с местната общност чрез презентации, контакти с действащи организации и координиране на анкети в общността, до формирането на група с политики, процедури и специално разработена ИТ система, която се превърна в пълноценно функционираща програма. Обхватът на предоставената подкрепа е особено  впечатляващ.",
                    Person = "Доброволец на СД",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Bulgarian"
                },
                new Testimonial()
                {
                    Tagline = "Много удовлетворяващо",
                    Message = "За мен удовлетворението е огромно – срещам нови хора и усвоявам нови умения.",
                    Person = "Доброволец на СД",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Bulgarian"
                },
                new Testimonial()
                {
                    Tagline = "Mano dienos dabar daug šviesesnės",
                    Message = "Dabar, kai mane lanko geros kaimynystės programos savanoris, mano dienos daug šviesesnės, nes anksčiau buvau gana vienišas. Jie taip pat padeda man nuvykti į vizitus ligoninėje, nes mano šeima gyvena toli.",
                    Person = "Geros kaimynystės programos paslaugų naudotojas",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Lithuanian"
                },
                new Testimonial()
                {
                    Tagline = "Žmonės, kuriems iš tiesų rūpi",
                    Message = "Negalėčiau apsieiti be savo gerųjų kaimynų. Šia paslauga daugiausia naudojuosi dėl transporto, tačiau parama ir draugystė yra fantastiška. Labai svarbu žinoti, kad yra žmonių, kuriems tikrai rūpi.",
                    Person = "Geros kaimynystės programos paslaugų naudotojas",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Lithuanian"
                },
                new Testimonial()
                {
                    Tagline = "Kavos rytai",
                    Message = "Man labai patinka dalyvauti gerų kaimynų rytinuose kavos susitikimuose; sutikau daug naujų žmonių ir susiradau draugų.",
                    Person = "Geros kaimynystės programos paslaugų naudotojas",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Lithuanian"
                },
                new Testimonial()
                {
                    Tagline = "Geriausias dalykas, kuris įvyko šioje vietovėje per daugelį metų",
                    Message = "Mūsų geros kaimynystės programa yra geriausias dalykas, kuris šioje vietovėje įvyko per daugelį metų. Man ramu, kad visada yra kas nors, kas gali padėti man reguliariai vykstant į skirtingas klinikas, ir labai džiaugiuosi reguliariais draugo apsilankymais. Tokios smulkmenos, kaip skambinimas telefonu mano vardu, yra labai svarbios.",
                    Person = "Geros kaimynystės programos paslaugų naudotojas",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Lithuanian"
                },
                new Testimonial()
                {
                    Tagline = "Vienas iš geriausių mano padarytų dalykų",
                    Message = "Geros kaimynystės programa – vienas geriausių dalykų, kuriuos esu padaręs savo gyvenime, nors jos organizavimas ir valdymas reikalauja daug pastangų ir laiko. Be „Community Lincs“ mūsų geros kaimynystės programa nebūtų organizuota. Ji taip pat nebūtų buvusi tokia patikima ir kompetentinga. „Community Lincs“ – žmonės, į kuriuos kreipiamės dėl paramos ir informacijos; be jų jaustumėmės pasimetę.",
                    Person = "Geros kaimynystės programos savanoris",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Lithuanian"
                },
                new Testimonial()
                {
                    Tagline = "Naujos draugystės",
                    Message = "Nuostabu girdėti iš vietos gyventojų, kurie jau užmezgė naujus ryšius ir draugystę, pasinaudoję geros kaimynystės programomis. Didžiuojamės galėdami finansuoti „Community Lincs“ organizaciją, kad ji galėtų toliau sėkmingai plėtoti šią veiklą, padėdama daugiau žmonių pasinaudoti šia parama ir jaustis mažiau atskirtiems. Nacionalinės loterijos žaidėjų dėka šios geros kaimynystės programos suvienys bendruomenes ir suteiks joms galimybę klestėti.",
                    Person = "Matt Poole, Nacionalinės loterijos bendruomenės fondo Midlands regiono finansavimo vyresnysis vadovas",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Lithuanian"
                },
                new Testimonial()
                {
                    Tagline = "Įspūdinga",
                    Message = "„Community Lincs“ įžiebė kibirkštį, kuri paskatino susidomėjimą mūsų geros kaimynystės programos kūrimu, ir nuolat teikė paramą nuo pradinių konsultacijų su vietos bendruomene, rengiant pristatymus, užmezgant ryšius su esamomis organizacijomis ir koordinuojant bendruomenės apklausą, iki įsteigtos grupės su politika, procedūromis ir specialiai sukurta IT sistema, kol galiausiai buvo sukurta visiškai veikianti programa. Suteiktos paramos mastas buvo ypač įspūdingas.",
                    Person = "Geros kaimynystės programos savanoris",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Lithuanian"
                },
                new Testimonial()
                {
                    Tagline = "Labai naudinga",
                    Message = "Man pačiam tai buvo labai naudinga – susipažinau su naujais žmonėmis ir įgijau naujų įgūdžių.",
                    Person = "Geros kaimynystės programos savanoris",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Lithuanian"
                },
                new Testimonial()
                {
                    Tagline = "Tagad manas dienas ir daudz gaišākas",
                    Message = "Tagad, kad mani apmeklē mans GNS brīvprātīgais, manas dienas ir daudz gaišākas, jo agrāk es biju diezgan vientuļa. Viņi arī palīdz man nokļūt uz vizītēm slimnīcā, jo mana ģimene dzīvo tālu prom.",
                    Person = "GNS pakalpojumu lietotājs",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Latvian"
                },
                new Testimonial()
                {
                    Tagline = "Cilvēki, kuriem patiešām rūp",
                    Message = "Es nevarētu iztikt bez saviem Labajiem kaimiņiem. Esmu izmantojusi šo pakalpojumu galvenokārt transporta vajadzībām, bet atbalsts un draudzība ir fantastiska. Tā ir milzīga atšķirība, zinot, ka ir cilvēki, kuriem patiešām rūp.",
                    Person = "Labo kaimiņu shēmas pakalpojumu lietotājs",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Latvian"
                },
                new Testimonial()
                {
                    Tagline = "Kafijas rīti",
                    Message = "Man ļoti patīk apmeklēt Labo kaimiņu kafijas rītus; esmu iepazinusi daudz jaunu cilvēku un ieguvusi draugus.",
                    Person = "GNS pakalpojumu lietotājs",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Latvian"
                },
                new Testimonial()
                {
                    Tagline = "Labākais, kas šajā apkaimē noticis daudzu gadu laikā",
                    Message = "Mūsu Labo kaimiņu programma ir labākais, kas šajā reģionā noticis daudzu gadu laikā. Tā dod man sirdsmieru, ka vienmēr ir kāds, kas var palīdzēt manos regulārajos braucienos uz dažādām klīnikām, un man ļoti patīk, ka mani regulāri apmeklē mans draugs. Nelieli darbi, piemēram, telefona zvanu veikšana manā vārdā, ir tik nozīmīgi.",
                    Person = "GNS pakalpojumu lietotājs",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Latvian"
                },
                new Testimonial()
                {
                    Tagline = "Viena no labākajām lietām, ko esmu darījis",
                    Message = "GNS ir viena no labākajām lietām, ko esmu darījis savā dzīvē, lai gan tās organizēšana un vadīšana prasīja daudz pūļu un laika. Bez Community Lincs mūsu Labo kaimiņu programma nepastāvētu. Mēs arī nebūtu tik pārliecināti un kompetenti. Community Lincs ir mūsu cilvēki, pie kuriem vērsties pēc atbalsta un informācijas; bez viņiem mēs justos pazuduši.",
                    Person = "Laba kaimiņa shēmas brīvprātīgais",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Latvian"
                },
                new Testimonial()
                {
                    Tagline = "Jaunas draudzības",
                    Message = "Ir brīnišķīgi dzirdēt no vietējiem iedzīvotājiem, kuri, pateicoties Labā kaimiņa shēmām, jau ir nodibinājuši jaunus kontaktus un draudzības. Mēs lepojamies, ka finansējam Community Lincs, lai tā varētu turpināt vairot šos panākumus, palīdzot vairāk cilvēkiem izmantot šo atbalstu un justies mazāk izolētiem. Pateicoties Nacionālās loterijas spēlētājiem, šīs Labo kaimiņu shēmas saliedēs kopienas, dodot tām iespēju uzplaukt.",
                    Person = "Mets Pūls (Matt Poole), Nacionālās loterijas kopienas fonda (National Lottery Community Fund) Midlendas reģiona finansēšanas nodaļas vecākais vadītājs",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Latvian"
                },
                new Testimonial()
                {
                    Tagline = "Iespaidīgi",
                    Message = "Community Lincs nodrošināja dzirksteli, kas radīja interesi par mūsu Labo kaimiņu shēmas izveidi, un ir sniegusi nepārtrauktu atbalstu, sākot no sākotnējām konsultācijām ar vietējo kopienu, izmantojot prezentācijas, sadarbojoties ar esošajām organizācijām un koordinējot kopienas aptauju, līdz pat grupas izveidei ar politiku, procedūrām un pielāgotu IT sistēmu, kas noslēdzas ar pilnībā funkcionējošas shēmas izveidi. Sniegtā atbalsta apjoms ir bijis īpaši iespaidīgs.",
                    Person = "GNS brīvprātīgais",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Latvian"
                },
                new Testimonial()
                {
                    Tagline = "Ļoti vērtīgs",
                    Message = "Man pašai tas ir devis lielu gandarījumu - esmu iepazinusi jaunus cilvēkus un apguvusi jaunas prasmes.",
                    Person = "GNS brīvprātīgais",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Latvian"
                },
                new Testimonial()
                {
                    Tagline = "Moje dni są teraz o wiele jaśniejsze",
                    Message = "Moje dni są teraz o wiele jaśniejsze, gdy odwiedza mnie wolontariusz GNS, ponieważ wcześniej byłam dość samotna. Wolontariusze pomagają mi także w dotarciu na wizyty w szpitalu, ponieważ moja rodzina mieszka daleko.",
                    Person = "Użytkownik usługi GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Polish"
                },
                new Testimonial()
                {
                    Tagline = "Ludzie, którym naprawdę zależy",
                    Message = "Nie poradziłabym sobie bez moich Dobrych Sąsiadów. Korzystam z tej usługi głównie ze względu na transport, ale oferowane wsparcie i przyjaźń są fantastyczne. To ogromna różnica wiedzieć, że są ludzie, którym naprawdę zależy\".",
                    Person = "Użytkownik usługi Programu Dobrego Sąsiada",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Polish"
                },
                new Testimonial()
                {
                    Tagline = "Poranki kawowe",
                    Message = "Bardzo lubię uczestniczyć w porankach kawowych dobrych sąsiadów; poznałam tam wiele nowych osób i zaprzyjaźniłam się z nimi.",
                    Person = "Użytkownik usługi GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Polish"
                },
                new Testimonial()
                {
                    Tagline = "Najlepsza rzecz, jaka wydarzyła się w tej okolicy od wielu lat",
                    Message = "Nasz Program Dobrego Sąsiada to najlepsza rzecz, jaka przytrafiła się w tej okolicy od wielu lat. Jestem spokojna, że zawsze jest osoba, które może mi pomóc przy moich wielu regularnych wizytach w różnych przychodniach i naprawdę cieszę się z regularnych wizyt mojego przyjaciela. Drobne rzeczy, jak na przykład wykonywanie telefonów w moim imieniu, mają tak duże znaczenie.",
                    Person = "Użytkownik usługi GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Polish"
                },
                new Testimonial()
                {
                    Tagline = "Jedna z najlepszych rzeczy, jakie zrobiłem",
                    Message = "GNS jest jedną z najlepszych rzeczy, jakie zrobiłem w swoim życiu, pomimo tego, że organizacja i zarządzanie nim wymaga znacznego wysiłku i czasu. Bez Community Lincs nasz Program Dobrego Sąsiada nie zostałby zorganizowany. Nie byłby też tak pewny i kompetentny. Community Lincs to nasi ludzie, do których możemy się udać po wsparcie i informacje; bez nich czulibyśmy się zagubieni.",
                    Person = "Wolontariusz Programu Dobrego Sąsiada",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Polish"
                },
                new Testimonial()
                {
                    Tagline = "Nowe przyjaźnie",
                    Message = "Wspaniale jest słuchać relacji mieszkańców, którzy już nawiązali nowe znajomości i przyjaźnie dzięki Programom Dobrego Sąsiada. Jesteśmy dumni, że możemy finansować Community Lincs i może się ono dalej z sukcesem rozwijać pomagając większej liczbie osób korzystać z tego wsparcia i czuć się mniej odizolowany. Dzięki graczom w National Lottery, te Programy Dobrego Sąsiada integrują społeczności, umożliwiając im rozkwit.",
                    Person = "Matt Poole, starszy kierownik ds. finansowania dla Midlands w National Lottery Community Fund",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Polish"
                },
                new Testimonial()
                {
                    Tagline = "Imponujące",
                    Message = "Community Lincs wykrzesała iskrę, która zapoczątkowała zainteresowanie ustanowieniem naszego Programu Dobrego Sąsiada i zapewniła stałe wsparcie od początkowych konsultacji z lokalną społecznością poprzez prezentacje, tworzenie sieci kontaktów z istniejącymi organizacjami i koordynację ankiety środowiskowej do utworzenia ukonstytuowanej grupy z zasadami, procedurami i dostosowanym systemem informatycznym, którego zwieńczeniem jest w pełni działający program. Zakres udzielonego wsparcia był szczególnie imponujący.",
                    Person = "Wolontariusz GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Polish"
                },
                new Testimonial()
                {
                    Tagline = "Bardzo satysfakcjonujące",
                    Message = "Korzyści dla mnie były bardzo satysfakcjonujące - poznawanie nowych ludzi i uczenie się nowych umiejętności.",
                    Person = "Wolontariusz GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Polish"
                },
                new Testimonial()
                {
                    Tagline = "Os meus dias são agora muito mais alegres",
                    Message = "Agora que o meu voluntário do GNS me visita, os meus dias são muito mais alegres, pois sentia-me muito sozinho. Também me ajudam a ir às minhas consultas no hospital, pois a minha família vive muito longe.",
                    Person = "Utilizador do Serviço GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Portugese"
                },
                new Testimonial()
                {
                    Tagline = "Pessoas que realmente se importam",
                    Message = "Eu não conseguiria dar conta de tudo sem os meus Bons Vizinhos. Tenho utilizado o serviço principalmente para transporte, mas o apoio e a amizade são fantásticos. Saber que há pessoas que realmente se importam faz uma grande diferença.",
                    Person = "Utilizador do Serviço Good Neighbour Scheme",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Portugese"
                },
                new Testimonial()
                {
                    Tagline = "Manhãs de café",
                    Message = "Gosto muito de participar nos eventos organizados pelos Bons Vizinhos, em que bebemos café de manhã. Conheci várias pessoas e fiz amigos.",
                    Person = "Utilizador do Serviço GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Portugese"
                },
                new Testimonial()
                {
                    Tagline = "A melhor coisa a acontecer nesta zona desde há muito tempo",
                    Message = "O nosso Good Neighbour Scheme é a melhor coisa que aconteceu nesta zona desde há muito tempo. Saber que há sempre alguém para me ajudar nas minhas deslocações periódicas à clínica dá-me paz de espírito, e gosto muito das visitas regulares do meu novo amigo. São as pequenas coisas, como fazer telefonemas em meu nome, que fazem toda a diferença.",
                    Person = "Utilizador do Serviço GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Portugese"
                },
                new Testimonial()
                {
                    Tagline = "Uma das melhores coisas que já fiz",
                    Message = "O GNS é uma das melhores coisas que fiz na vida, apesar de a sua organização e gestão levar um esforço e tempo consideráveis. Sem a Community Lincs, o nosso Good Neighbour Scheme não teria sido organizado. Nem teria tanta confiança e competência. A Community Lincs é constituída pelo nosso pessoal, a quem recorremos para obter apoio e informação. Sem eles, estaríamos perdidos.",
                    Person = "Voluntário do Good Neighbour Scheme",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Portugese"
                },
                new Testimonial()
                {
                    Tagline = "Novas amizades",
                    Message = "É maravilhoso ouvir os residentes locais dizer que já estabeleceram novas ligações e amizades graças aos Good Neighbour Schemes. É com orgulho que financiamos a Community Lincs para que esta possa continuar a crescer, ajudando mais pessoas a tirar partido deste apoio e sentir-se menos isoladas. Graças aos jogadores da Lotaria Nacional, estes Good Neighbour Schemes irão unir as comunidades, ajudando-as a prosperar.",
                    Person = "Matt Poole, responsável pelo financiamento para as Midlands do Fundo Comunitário da Lotaria Nacional",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Portugese"
                },
                new Testimonial()
                {
                    Tagline = "Impressionante",
                    Message = "A Community Lincs produziu a faísca que despertou o interesse na criação do nosso Good Neighbour Scheme e tem oferecido um apoio contínuo desde a consulta inicial com a comunidade local, através de apresentações, interações com organizações existentes e coordenação de um inquérito comunitário para a formação de um grupo constituído com políticas, procedimentos e um sistema de TI personalizado, culminando num programa totalmente operacional. A envergadura do apoio prestado tem sido particularmente impressionante.",
                    Person = "Voluntário do GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Portugese"
                },
                new Testimonial()
                {
                    Tagline = "Muito gratificante",
                    Message = "As vantagens para mim têm sido muito gratificantes: conhecer pessoas novas e adquirir novas competências.",
                    Person = "Voluntário do GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Portugese"
                },new Testimonial()
                {
                    Tagline = "Zilele mele au devenit mult mai luminoase",
                    Message = "Zilele mele au devenit mult mai luminoase acum, când mă vizitează voluntarul proiectului de bună vecinătate (GNS), înainte aveam o viață destul de singuratică. De asemenea mă ajută să ajung la programările la spital, având în vedere că familia mea locuiește la o distanță mare.",
                    Person = "Utilizator al serviciilor asigurate în cadrul Proiectului de bună vecinătate (GNS)",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Romanian"
                },
                new Testimonial()
                {
                    Tagline = "Oameni cărora chiar le pasă",
                    Message = "Nu m-aș descurca fără voluntarii din Proiectul de bună vecinătate. Am apelat la serviciile lor în principal pentru transport, însă sprijinul oferit și prietenia sunt fantastice. Este nemaipomenit de important să știu că există oameni cărora chiar le pasă.",
                    Person = "Utilizator al serviciilor asigurate în cadrul Proiectului de bună vecinătate",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Romanian"
                },
                new Testimonial()
                {
                    Tagline = "Dimineața la o cafea",
                    Message = "Îmi place foarte mult să-mi petrec diminețile la o cafea în cadrul proiectului de bună vecinătate; am cunoscut multe persoane noi și mi-am făcut prieteni.",
                    Person = "Utilizator al serviciilor asigurate în cadrul Proiectului de bună vecinătate (GNS)",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Romanian"
                },
                new Testimonial()
                {
                    Tagline = "Cel mai bun lucru care s-a întâmplat în zonă de mulți ani",
                    Message = "Proiectul nostru de bună vecinătate este cel mai bun lucru întâmplat în această zonă de mulți ani. Mă liniștește gândul că există mereu cineva care să mă ajute cu numeroasele mele vizite regulate la diverse clinici și mă bucur sincer de vizitele periodice pe care mi le face voluntarul prieten. Lucruri mărunte, precum faptul că dă telefoane în numele meu, fac o mare diferență.",
                    Person = "Utilizator al serviciilor asigurate în cadrul Proiectului de bună vecinătate (GNS)",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Romanian"
                },
                new Testimonial()
                {
                    Tagline = "Unul dintre cele mai bune lucruri pe care le-am făcut",
                    Message = "GNS este unul dintre cele mai bune lucruri pe care le-am făcut vreodată, în ciuda faptului că organizarea și gestionarea proiectului presupun dedicarea unor eforturi și investiții de timp considerabile. Dacă nu ar fi existat Community Lincs, Proiectul nostru de bună vecinătate nu ar fi fost organizat, nici nu ar fi fost la fel de încrezător și de avizat. Community Lincs înseamnă oamenii cărora ne putem adresa pentru asistență și informații; fără ei, ne-am simți pierduți.",
                    Person = "Voluntar în cadrul Proiectului de bună vecinătate",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Romanian"
                },
                new Testimonial()
                {
                    Tagline = "Prietenii noi",
                    Message = "Este minunat să auzim de la localnici care au creat deja relații și prietenii noi datorită proiectelor de bună vecinătate. Suntem mândri că finanțăm Community Lincs astfel încât să se poată baza în continuare pe această reușită, ajutând mai mulți oameni să se bucure de acest sprijin și să se simtă mai puțin izolați. Mulțumită participanților la loteria națională, aceste proiecte de bună vecinătate vor reuni comunitățile, dându-le posibilitatea de a prospera.",
                    Person = "Matt Poole, coordonator principal al programului de finanțare pentru regiunea Midlands în cadrul Fondului Comunitar al Loteriei Naționale",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Romanian"
                },
                new Testimonial()
                {
                    Tagline = "Impresionant",
                    Message = "Community Lincs a adus scânteia care a aprins interesul pentru înființarea proiectului nostru de bună vecinătate și a oferit sprijin continuu de la consultarea inițială cu membrii comunității locale, apoi la prezentări, întărirea relațiilor cu organizațiile existente și coordonarea unui sondaj în cadrul comunității până la formarea unui grup stabil cu politici, proceduri și un sistem IT special, culminând cu un proiect pe deplin operațional. Amploarea sprijinului asigurat a fost deosebit de impresionantă.",
                    Person = "Voluntar în cadrul GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Romanian"
                },
                new Testimonial()
                {
                    Tagline = "O experiență foarte satisfăcătoare",
                    Message = "Pentru mine, este o experiență foarte satisfăcătoare – am cunoscut persoane noi și am deprins abilități noi.",
                    Person = "Voluntar în cadrul GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Romanian"
                },
                new Testimonial()
                {
                    Tagline = "Теперь мои дни стали намного светлее",
                    Message = "Мои дни стали намного светлее, с тех пор как меня навещает волонтер программы добрососедства (GNS), ведь раньше мне было достаточно одиноко. Они также помогают мне добираться на приемы в больницу, поскольку моя семья живет далеко.",
                    Person = "Пользователь услуг GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Russian"
                },
                new Testimonial()
                {
                    Tagline = "Люди, которым действительно не все равно",
                    Message = "Я бы не смог обходиться без моих добрых соседей. Я пользовался услугами службы в основном для транспортировки, но поддержка и дружба просто фантастические. Очень важно знать, что рядом есть люди, которым не все равно.",
                    Person = "Пользователь услуг программы добрососедства",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Russian"
                },
                new Testimonial()
                {
                    Tagline = "Утренние встречи за чашкой кофе",
                    Message = "Мне очень нравится посещать утренние встречи за чашкой кофе для добрых соседей; я познакомился со многими новыми людьми и завел друзей.",
                    Person = "Пользователь услуг GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Russian"
                },
                new Testimonial()
                {
                    Tagline = "Лучшее, что произошло в этом районе за многие годы",
                    Message = "Наша программа добрососедства — это лучшее, что произошло в этом районе за многие годы. Она дает мне уверенность в том, что всегда есть кто-то, готовый помочь мне с регулярными походами в различные клиники, и мне очень нравятся регулярные визиты моего помощника. Такие мелочи, как телефонные звонки от моего имени, имеют большое значение.",
                    Person = "Пользователь услуг GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Russian"
                },
                new Testimonial()
                {
                    Tagline = "Одна из лучших вещей, которые я сделал",
                    Message = "GNS — одна из лучших вещей, которые я сделал в своей жизни несмотря на то, что ее организация и управление требуют значительных усилий и времени. Без Community Lincs наша программа добрососедства не была бы организована. Она также не была бы столь уверенной и компетентной. Community Lincs — это люди, к которым мы обращаемся за поддержкой и информацией; без них мы бы чувствовали себя потерянными.",
                    Person = "Волонтер Программы добрососедства",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Russian"
                },
                new Testimonial()
                {
                    Tagline = "Новые дружеские отношения",
                    Message = "Приятно слышать отзывы от местных жителей, которые уже завели новые связи и дружбу благодаря программам добрососедства. Мы гордимся тем, что финансируем Community Lincs, чтобы она могла продолжать развивать этот успех, помогая большему количеству людей воспользоваться этой поддержкой и почувствовать себя менее изолированными. Благодаря игрокам Национальной лотереи эти программы добрососедства объединят сообщества и позволят им процветать.",
                    Person = "Мэтт Пул, старший руководитель отдела финансирования для центральных графств в Фонде сообществ Национальной лотереи",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Russian"
                },
                new Testimonial()
                {
                    Tagline = "Впечатляюще",
                    Message = "Community Lincs послужила той искрой, которая зажгла интерес к созданию нашей программы добрососедства, и обеспечила постоянную поддержку, начиная первоначальными консультациями с местным сообществом посредством презентаций, налаживания связей с существующими организациями и координации опроса сообщества и заканчивая формированием группы с политикой, процедурами и специально разработанной ИТ-системой, что привело к созданию полностью функционирующей программы. Особенно впечатляет широта предоставляемой поддержки.",
                    Person = "Волонтер GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Russian"
                },
                new Testimonial()
                {
                    Tagline = "Очень полезно",
                    Message = "Для меня самого знакомство с новыми людьми и приобретение новых навыков было очень полезно.",
                    Person = "Волонтер GNS",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "boston",
                    Language = "Russian"
                }

            };

            return messageList;
        }

        public async Task<bool> PostRecordFeedback(int jobId, RequestRoles requestRoles, int? userId, FeedbackRating feedbackRating)
        {
            PostRecordFeedbackRequest request = new PostRecordFeedbackRequest()
            {
                JobId = jobId,
                RequestRoleType = new RequestRoleType() { RequestRole = requestRoles },
                UserId = userId,
                FeedbackRatingType = new FeedbackRatingType() { FeedbackRating = feedbackRating}
            };
            string json = JsonConvert.SerializeObject(request);
            StringContent data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await Client.PostAsync("/api/PostRecordFeedback", data);
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<PostRecordFeedbackResponse, FeedbackServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content.Success;
            }
            return false;
        }
    }
}
