using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Models.Validation;
using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    [RequestStageValidation]
    public class RequestHelpRequestStageViewModel : IRequestHelpStageViewModel
    {
        public string PageHeading { get; set; }
        public string IntoText { get; set; }
        public List<TasksViewModel> Tasks { get; set; }
        public List<RequestorViewModel> Requestors { get; set; }
        public QuestionsViewModel Questions { get; set; } = new QuestionsViewModel();

        public string TemplateName { get; set; } = "RequestHelpRequestStageViewModel";
        public List<RequestHelpTimeViewModel> Timeframes { get; set; }
        public bool? IsHealthCritical
        {
            get
            {
                if (Questions == null || Questions.Questions == null) return null;
                var healthCriticalQuestion = Questions.Questions.Where(x => x.ID == (int)HelpMyStreet.Utils.Enums.Questions.IsHealthCritical).FirstOrDefault();
                if (healthCriticalQuestion == null || healthCriticalQuestion.Model == null) return null; 
                return bool.Parse(healthCriticalQuestion.Model);
            }
        }
        public bool AgreeToTerms { get; set; }
        public bool AgreeToPrivacy { get; set; }
    }
}

