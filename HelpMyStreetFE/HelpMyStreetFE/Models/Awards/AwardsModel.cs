using System;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Awards
{
    public class AwardsModel
    {
        public string AwardName { get; set; }
        public int AwardValue { get; set; }
        public string AwardDescription { get; set; }
        public string ImageLocation { get; set; } = "/img/icons/thumbs-up.jpg";
    }
}
