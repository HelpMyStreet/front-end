using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class QuestionsViewModel
    {
        public List<RequestHelpQuestion> Questions { get; set; }

        public QuestionsViewModel GetQuestionsByLocation(string location)
        {
            QuestionsViewModel questionsViewModel = new QuestionsViewModel();

            if (Questions != null)
            {
                questionsViewModel.Questions = Questions.Where(a => a.Location == location).ToList();
            }

            return questionsViewModel;
        }
    }
}
