using HelpMyStreet.Contracts;
using HelpMyStreet.Contracts.FeedbackService.Request;
using HelpMyStreet.Contracts.FeedbackService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Feedback;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
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

        public async Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId)
        {
            string url = "/api/GetNewsTicker";

            if (groupId.HasValue)
            {
                url += $"?groupId={groupId.Value}";
            }


            HttpResponseMessage response = await Client.GetAsync(url);
            string str = await response.Content.ReadAsStringAsync();
            var deserializedResponse = JsonConvert.DeserializeObject<ResponseWrapper<NewsTickerResponse, FeedbackServiceErrorCode>>(str);
            if (deserializedResponse.HasContent && deserializedResponse.IsSuccessful)
            {
                return deserializedResponse.Content.Messages;
            }
            throw new Exception($"Bad response from GetNewsTicker for groupId {groupId}");
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
