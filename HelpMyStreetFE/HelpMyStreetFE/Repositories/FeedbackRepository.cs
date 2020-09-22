using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Feedback;
using HelpMyStreetFE.Services;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace HelpMyStreetFE.Repositories
{
    public class FeedbackRepository : IFeedbackRepository
    {
        public FeedbackRepository()
        {
        }

        public async Task<List<FeedbackMessage>> GetFeedback()
        {
            var messageList = new List<FeedbackMessage>()
            {
                new FeedbackMessage()
                {
                    Tagline = "Huge thank-you",
                    Message = "All of the staff here at the Nottinghamshire YMCA would like to say a huge thank you for the homemade face coverings provided by volunteers from HelpMyStreet. They're well-made and all different which makes them much more interesting than normal disposable masks! We're a charity and requested 100 face coverings as a donation to help our staff and residents stay safe during this pandemic.",
                    Person = "Gillian from Nottingham asked for help",
                    Type = FeedbackMessageType.FaceCovering,
                    B2BFeedback = true,
                },
                new FeedbackMessage()
                {
                    Tagline = "Lovely chat",
                    Message = "I volunteered to help out with the Age UK Vitals for Veterans campaign. Delivering the wellbeing package was very easy, I collected it from my local Age UK branch and was told where to drop it off. I had a lovely chat with the recipient of the parcel, an RAF veteran who was out tending to his garden when I arrived. I'm looking forward to the next delivery!",
                    Person = "Sean in Boston provided some help",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuklsl"
                },
                new FeedbackMessage()
                {
                    Tagline = "Grateful thanks",
                    Message = "The masks have arrived and are really lovely, very comfortable and of very good quality. Please pass on our grateful thanks to all that took the time to sew these masks. Especially to the wonderful Gill who very kindly made 60 of the masks!",
                    Person = "Ann-Marie from Boston asked for help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Excellent service",
                    Message = "Just want to say as soon as I requested the masks they were in process to be made. Delivered within 2 days and excellent quality for adults and kids. Thank you so much at a difficult financial time I managed to purchase for the whole family too. Excellent service.",
                    Person = "Gurmeet from Leicester asked for help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Really great",
                    Message = "The process of requesting face coverings through HelpMyStreet couldn’t have been easier. I requested four face coverings for my children and mentioned the mixture of boys and girls. Within a week 4 really stylish face coverings had been posted to our home and the children were thrilled with the designs (Harry Potter etc). I wouldn’t hesitate to recommend HelpMyStreet or Collette who made the face coverings. Not only were the face coverings really great but it also feels reassuring that our donation will cover Collette’s cost but also go to charity.",
                    Person = "Mark from Coventry asked for help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "A real boost!",
                    Message = "I was only able to make a few face masks, but it was good to know that they were going to people who needed them. It showed me that you don't need to do a huge amount of work to really help others. I had a lovely thank you message from one recipient and that was a real boost!",
                    Person = "Sue in Chesterfield provided some help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Totally happy",
                    Message = "I was extremely grateful and totally happy with the masks that were made for myself, my granddaughter and my son. They fitted us perfectly and most importantly my 9 year old granddaughter was comfortable, proud & happy wearing hers for a long trip to France on Eurostar. She got to choose the material pattern thanks to Kiyomi’s communication with me. Praise to all 😘",
                    Person = "Sue from Hornchurch asked for help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Delighted",
                    Message = "I am delighted with my face coverings, they are perfect and they fit both my husband and I perfectly. Thank you so much to the lovely person who made them.  They've washed well too.",
                    Person = "Sue from Llandudno asked for help",
                    Type = FeedbackMessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Can't recommend it enough",
                    Message = "HelpMyStreet has allowed us to keep what was great about our COVID-19 response, but set ourselves up to keep this going even when things hopefully get back to being a bit more normal. We can’t recommend it highly enough.",
                    Person = "Dave is a Community Organiser in Tankersley",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "tankersley",
                    B2BFeedback = true,
                },
                new FeedbackMessage()
                {
                    Tagline = "Absolute community spirit",
                    Message = "Big 'Thank you from the Barkantine Practice', was lovely to receive these masks today and give them to the staff. So good to see a range of colours and sizes and materials and also catering for the boys, we are truly grateful.  Thanks for all the work you are doing, absolute community spirit at its best.",
                    Person = "Sasha is a GP in Tower Hamlets who requested face coverings to help staff commute on public transport",
                    Type = FeedbackMessageType.FaceCovering,
                    B2BFeedback = true,
                },
                new FeedbackMessage()
                {
                    Tagline = "Local people",
                    Message = "The ability to shape the content and presentation of local material - on the website and with the posters and flyers - makes it much more likely that local people will access and use this service",
                    Person = "Kate organises a local group in South Yorkshire who built a community page on HelpMyStreet",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "tankersley",
                    B2BFeedback = true,
                },
                new FeedbackMessage()
                {
                    Tagline = "A privilege",
                    Message = "It has been a privilege to join HelpMyStreet and to deliver the wellbeing packages for Age UK. To be able to give something back to others who have given to us in the past is most rewarding.",
                    Person = "Sharon in Stamford provided some help",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuklsl"
                },
                new FeedbackMessage()
                {
                    Tagline = "Very rewarding",
                    Message = "I am enjoying being a volunteer for the Age UK Vitals for Veterans campaign. Helping those who have served their country is very rewarding and I am only too pleased to be able to help.",
                    Person = "Steve in Spalding provided some help",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuklsl"
                },
                new FeedbackMessage()
                {
                    Tagline = "A fantastic experience",
                    Message = "Working with the team from Factor 50 on the HelpMyStreet initiative has been a fantastic experience.  We have developed a wonderful partnership which has genuinely helped us to reach so many more older people and willing volunteers alike for our Vitals for Veterans project.  They are so flexible and engaged with us that we are now also looking at other ways that we can work with them on different services and projects we have.",
                    Person = "Michele Jolly, CEO at Age UK LSL",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuklsl",
                    B2BFeedback = true,
                },
                new FeedbackMessage()
                {
                    Tagline = "A valuable tool",
                    Message = "Using HelpMyStreet enables us to recruit volunteers and support older people in communities that are sometimes harder to reach. It’s a valuable tool in a rural county.",
                    Person = "Nicki Lee, Senior Volunteer Coordinator at Age UK LSL",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ageuklsl",
                    B2BFeedback = true,
                 },
                new FeedbackMessage()
                {
                    Tagline = "A wonderful village",
                    Message = "Thank you from the bottom of my heart to you and the team for helping my parents through this difficult time. It's so appreciated. What you guys did and will continue to do was/is amazing. Ruddington is a wonderful village ❤",
                    Person = "Leigh requested help for her parents is Ruddington",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ruddington",
                 },
                new FeedbackMessage()
                {
                    Tagline = "A caring bunch",
                    Message = "I am proud and pleased to help with the Covid-19 group in my village. There was an obvious need for our services and we came together as a community in a brilliant way. Lots of people volunteered to assist without hesitation which just shows what a caring bunch we are.",
                    Person = "Pam from Ruddington provided help",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ruddington",
                 },
                new FeedbackMessage()
                {
                    Tagline = "Really keen to get involved",
                    Message = "When I heard that Ruddington was in need of volunteers to assist with the Covid response I was really keen to get involved. I have children at both local schools and it was great to be able to deliver schoolwork out to their classmates who were stuck at home. Although they knew what it meant when they saw me walking towards the door, most of the kids were excited to be getting a delivery from school and to see what fresh challenges had been set by their teachers.",
                    Person = "Harvey from Ruddington provided help",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ruddington",
                 },
                new FeedbackMessage()
                {
                    Tagline = "A poem of thanks!",
                    Message = "Thank you to our local Pub, Thank you for all for our delicious grub, Thank you to everyone involved in the Ruddington Covid 19 Mutual Aid Group, Thank you also for keeping us all in the Loop, For arranging this, Fabulous dish xx",
                    Person = "Pat and George wanted to say thanks",
                    Type = FeedbackMessageType.Group,
                    GroupKey = "ruddington",
                 },
            };

            return messageList;
        }
        
    }
}
