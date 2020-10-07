using System;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Awards
{
    public class CurrentAwardModel
    {
        public AwardsModel Award { get; set; }
        public int CurrentAwardLevel { get; set; }
        public int NextAwardLevel { get; set; }

    }
}
