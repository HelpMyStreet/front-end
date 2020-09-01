using System;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Awards
{
    public enum AwardsDescriptionModifiers
    {
        StandardDescription,
        CountListDescription
    }
    public class AwardsModel
    {
        private string _awardDescription;
        public int JobCount { get; set; }
        public string JobDetail { get; set; }
        public string AwardName { get; set; }
        public int AwardValue { get; set; }
        public string AwardDescription { set { _awardDescription = value; } get
            {
                if (JobCount == 0 || JobDetail == "")
                {
                    return _awardDescription;
                }
                else
                {
                    return DescriptionModifier(JobCount, JobDetail, _awardDescription);
                }
            } }
        public string ImageLocation { get; set; } = "/img/icons/thumbs-up.svg";
        public Func<List<Object>, bool> SpecificPredicate { get; set; } = x => true;
        public Func<int, string, string, string> DescriptionModifier { get; set; } = (x, y, z) => z;
        //public Func<int, string, string> AwardDescription { get; set; }

        public AwardsModel(){
            JobCount = 0;
            JobDetail = "";
            AwardDescription = "";
        }
}
}
