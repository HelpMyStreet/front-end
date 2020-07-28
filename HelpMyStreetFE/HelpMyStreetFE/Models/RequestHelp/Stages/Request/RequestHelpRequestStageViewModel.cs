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
        public QuestionsViewModel RequestHelpQuestions { get; set; }

        public string TemplateName { get; set; } = "RequestHelpRequestStageViewModel";
        public List<RequestHelpTimeViewModel> Timeframes { get; set; }
        public bool? IsHealthCritical
        {
            get
            {
                var selectedTask = this.Tasks.Where(x => x.IsSelected).FirstOrDefault();
                if (selectedTask == null) return null;
                var healthCriticalQuestion = selectedTask.Questions.Where(x => x.ID == (int)Questions.IsHealthCritical).FirstOrDefault();
                if (healthCriticalQuestion == null || healthCriticalQuestion.Model == null) return null; 
                return bool.Parse(healthCriticalQuestion.Model);
            }
        }
        public bool AgreeToTerms { get; set; }
        public bool AgreeToPrivacy { get; set; }

        public QuestionsViewModel GetRequestHelpQuestionsByLocation(string location)
        {
            QuestionsViewModel questionsViewModel = new QuestionsViewModel();

            if (RequestHelpQuestions != null)
            {
                questionsViewModel.Questions = RequestHelpQuestions.Questions.Where(a => a.Location == location).ToList();
            }

            return questionsViewModel;
        }
    }
}

