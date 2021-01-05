using System;
using System.Collections.Generic;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Awards
{
    public class CurrentAwardModel
    {
        public AwardsModel Award { get; set; }
        public int CompletedJobCount { get; set; }
        public Dictionary<SupportActivities, int> CompletedJobs { get; set; }
    }
}
