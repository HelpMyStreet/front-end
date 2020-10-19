using System;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Awards
{
    public class AwardsModel
    {
        private string _awardDescription;
        public Dictionary<string,Object> AwardAttributes { get; set; }
        public string AwardName { get; set; }
        public int AwardValue { get; set; }
        public string AwardDescription { set { _awardDescription = value; } get
            {
                if (AwardAttributes.Count == 0)
                {
                    return _awardDescription;
                }
                else
                {
                    return DescriptionModifier(AwardAttributes, _awardDescription);
                }
            } }
        public string ImageLocation { get; set; } = "/img/icons/thumbs-up.svg";
        public Func<List<Object>, bool> SpecificPredicate { get; set; } = x => true;
        public Func<Dictionary<string, Object>, string, string> DescriptionModifier { get; set; } = (x, z) => z;

        public AwardsModel(){
            AwardAttributes = new Dictionary<string, Object>();
            AwardDescription = "";
        }
}
}
