using System;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Awards
{
    public class AwardsModel
    {
        public int AwardValue { get; set; }
        public string AwardDescription { get; set; }
        public string ImageLocation { get; set; } = "/img/icons/thumbs-up.svg";
        public Func<List<Object>, bool> SpecificPredicate { get; set; } = x => true;
    }
}
