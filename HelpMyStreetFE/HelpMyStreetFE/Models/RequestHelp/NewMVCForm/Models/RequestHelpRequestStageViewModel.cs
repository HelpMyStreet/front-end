using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Interface;
using System.Collections.Generic;


namespace HelpMyStreetFE.Models.RequestHelp.NewMVCForm.Models
{
    public class RequestHelpRequestStageViewModel : IRequestHelpStepsViewModel
    {
 
        public List<TasksViewModel> Tasks { get; set; }
        public List<RequestorViewModel> Requestors { get; set; }          

        public string TemplateName { get; set; } = "RequestHelpRequestStageViewModel";
        public List<RequestHelpTimeViewModel> Timeframes { get; set; }
        public bool? IsHealthCritical { get; set; }
        public bool AgreeToTerms { get; set; }
        public bool AgreeToPrivacy { get; set; }

    }

    public class RequestorViewModel
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string IconDark { get; set; }
        public string IconLight { get; set; }
        public string ColourCode { get; set; }     
        public bool IsSelected { get; set; }
        public RequestorType Type { get; set; }
    }

    public enum RequestorType
    {
        OnBehalf,
        Myself
    }
    public class TasksViewModel
    {
        public int ID { get; set; }
        public SupportActivities SupportActivity { get; set; }
        public List<RequestHelpQuestion> Questions { get; set; }

        public bool IsSelected { get; set; }
    }

    public class RequestHelpQuestion
    {
        public int ID { get; set; }
        public string Label { get; set; }
        public InputType InputType { get; set; }
        public dynamic Model { get; set; }
    }

    public enum InputType
    {
        Textarea = 1,
        Textbox = 2,
        Number = 3,
    }

  


}

