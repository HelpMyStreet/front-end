using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.RequestHelp.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp
{
    public class RequestHelpQuestion
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public QuestionType InputType { get; set; }
        public string Model { get; set; }
        public bool Required { get; set; }
        public string Location { get; set; }
        public string SubText { get; set; }
        public string PlaceholderText { get; set; }
        public int? Max { get; set; }
        public int? MaxLength { get; set; }
        public List<AdditonalQuestionData> AdditionalData { get; set; }
        public string DataValidationMessage { get
            {
                return InputType switch
                {
                    QuestionType.Radio => "Please select from one of the available options",
                    QuestionType.Number => "Please enter a number",
                    QuestionType.Text => MaxLength.HasValue ? $"Please enter a string between 1 and {MaxLength.Value} characters" : "Please enter a value",
                    QuestionType.MultiLineText => "Please enter a value",
                    QuestionType.LabelOnly => "This shouldn't happen",
                    _ => "Please enter a value"
                };                                
            } 
        }
    }

 
}
