using HelpMyStreetFE.Models.Home;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models
{
    public class HomeViewModel
    {
        public bool isLoggedIn { get; set; }
        public bool TestBanner { get; set; }
        public List<FeedbackMessage> FeedbackMessages { get; set; }
    }
}
