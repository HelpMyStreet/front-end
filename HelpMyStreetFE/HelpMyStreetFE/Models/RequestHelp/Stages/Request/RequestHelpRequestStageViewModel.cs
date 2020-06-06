using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class RequestHelpRequestStageViewModel : IRequestHelpStageViewModel
    {
 
        public List<TasksViewModel> Tasks { get; set; }
        public List<RequestorViewModel> Requestors { get; set; }          

        public string TemplateName { get; set; } = "RequestHelpRequestStageViewModel";
        public List<RequestHelpTimeViewModel> Timeframes { get; set; }
        public bool? IsHealthCritical
        {
            get
            {
                var selectedTask = this.Tasks.Where(x => x.IsSelected).FirstOrDefault();
                if (selectedTask == null) return null;
                var healthCriticalQuestion = selectedTask.Questions.Where(x => x.ID == (int)HelpMyStreet.Utils.Enums.Questions.IsHealthCritical).FirstOrDefault();
                if (healthCriticalQuestion == null) return null; 
                return "yes".Equals(healthCriticalQuestion.Model.ToLower()); // if we dont ask the question then its not health critical;

            }
        }
        public bool AgreeToTerms { get; set; }
        public bool AgreeToPrivacy { get; set; }

    }
}

