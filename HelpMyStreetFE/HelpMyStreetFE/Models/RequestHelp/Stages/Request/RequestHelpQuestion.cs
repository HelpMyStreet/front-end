using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class RequestHelpQuestion
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public QuestionType InputType { get; set; }
        public string Model { get; set; }
        public bool Required { get; set; }        
        public List<AdditonalQuestionData> AdditionalData { get; set; }
        public bool DontShow { get; set; }
    }

 
}
