using HelpMyStreet.Utils.Utils;
using HelpMyStreetFE.Models;
using HelpMyStreetFE.Models.Home;
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
                    Person = "Gillian from Nottingham asked for help,\r\nJune 2020",
                    Type = MessageType.FaceCovering
                  
                },
                new FeedbackMessage()
                {
                    Tagline = "Lovely chat",
                    Message = "I volunteered to help out with the Age UK Vitals for Veterans campaign. Delivering the wellbeing package was very easy, I collected it from my local Age UK branch and was told where to drop it off. I had a lovely chat with the recipient of the parcel, an RAF veteran who was out tending to his garden when I arrived. I'm looking forward to the next delivery!",
                    Person = "Sean in Boston provided some help,\r\nJuly 2020",
                    Type = MessageType.Group
                },
                new FeedbackMessage()
                {
                    Tagline = "Grateful thanks",
                    Message = "The masks have arrived and are really lovely, very comfortable and of very good quality. Please pass on our grateful thanks to all that took the time to sew these masks. Especially to the wonderful Gill who very kindly made 60 of the masks!",
                    Person = "Ann-Marie from Boston asked for help,\r\nJune 2020",
                    Type = MessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Excellent service",
                    Message = "Just want to say as soon as I requested the masks they were in process to be made. Delivered within 2 days and excellent quality for adults and kids. Thank you so much at a difficult financial time I managed to purchase for the whole family too. Excellent service.",
                    Person = "Gurmeet from Leicester asked for help,\r\nJuly 2020",
                    Type = MessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Really great",
                    Message = "The process of requesting face coverings through HelpMyStreet couldn’t have been easier. I requested four face coverings for my children and mentioned the mixture of boys and girls. Within a week 4 really stylish face coverings had been posted to our home and the children were thrilled with the designs (Harry Potter etc). I wouldn’t hesitate to recommend HelpMyStreet or Collette who made the face coverings. Not only were the face coverings really great but it also feels reassuring that our donation will cover Collette’s cost but also go to charity.",
                    Person = "Mark from Coventry asked for help,\r\nJuly 2020",
                    Type = MessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "A real boost!",
                    Message = "I was only able to make a few face masks, but it was good to know that they were going to people who needed them. It showed me that you don't need to do a huge amount of work to really help others. I had a lovely thank you message from one recipient and that was a real boost!",
                    Person = "Sue in Chesterfield provided some help,\r\nJune 2020",
                    Type = MessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Totally happy",
                    Message = "I was extremely grateful and totally happy with the masks that were made for myself, my granddaughter and my son. They fitted us perfectly and most importantly my 9 year old granddaughter was comfortable, proud & happy wearing hers for a long trip to France on Eurostar. She got to choose the material pattern thanks to Kiyomi’s communication with me. Praise to all 😘",
                    Person = "Sue from Hornchurch asked for help,\r\nJuly 2020",
                    Type = MessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Delighted",
                    Message = "I am delighted with my face coverings, they are perfect and they fit both my husband and I perfectly. Thank you so much to the lovely person who made them.  They've washed well too.",
                    Person = "Sue from Llandudno asked for help,\r\nJuly 2020",
                    Type = MessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Can't recommend it enough",
                    Message = "HelpMyStreet has allowed us to keep what was great about our COVID-19 response, but set ourselves up to keep this going even when things hopefully get back to being a bit more normal. We can’t recommend it highly enough.",
                    Person = "Dave is a Community Organiser in Tankersley,\r\nJuly 2020",
                    Type = MessageType.Group
                },
                new FeedbackMessage()
                {
                    Tagline = "Absolute community spirit",
                    Message = "Big 'Thank you from the Barkantine Practice', was lovely to receive these masks today and give them to the staff. So good to see a range of colours and sizes and materials and also catering for the boys, we are truly grateful.  Thanks for all the work you are doing, absolute community spirit at its best.",
                    Person = "Sasha is a GP in Tower Hamlets who requested face coverings to help staff commute on public transport,\r\nJuly 2020",
                    Type = MessageType.FaceCovering
                },
                new FeedbackMessage()
                {
                    Tagline = "Local people",
                    Message = "The ability to shape the content and presentation of local material - on the website and with the posters and flyers - makes it much more likely that local people will access and use this service",
                    Person = "Kate organises a local group in South Yorkshire who built a community page on HelpMyStreet,\r\nJuly 2020",
                    Type = MessageType.Group
                }

            };

            return messageList;
        }
        
    }
}
