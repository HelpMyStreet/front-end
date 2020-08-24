using System;
namespace HelpMyStreetFE.Models.Awards
{
    public class AwardsViewModel
    {
        public AwardsModel Award { get; set; }
        public int CurrentAwardLevel { get; set; }
        public int NextAwardLevel { get; set; }
    }
}
