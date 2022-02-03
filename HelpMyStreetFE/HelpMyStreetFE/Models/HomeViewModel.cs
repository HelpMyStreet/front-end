using HelpMyStreet.Contracts;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Home;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models
{
    public class HomeViewModel
    {
        public bool isLoggedIn { get; set; }
        public IEnumerable<NewsTickerMessage> NewsTickerMessages { get; set; }
    }
}
