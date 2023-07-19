using HelpMyStreet.Contracts;
using HelpMyStreet.Contracts.FeedbackService.Request;
using HelpMyStreet.Contracts.FeedbackService.Response;
using HelpMyStreet.Contracts.Shared;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
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
                    Tagline = "Absolute community spirit",
                    Message = "Big 'Thank you from the Barkantine Practice', was lovely to receive these masks today and give them to the staff. So good to see a range of colours and sizes and materials and also catering for the boys, we are truly grateful.  Thanks for all the work you are doing, absolute community spirit at its best.",
                    Person = "Sasha is a GP in Tower Hamlets who requested face coverings to help staff commute on public transport",
                    Type = FeedbackMessageType.FaceCovering,
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
