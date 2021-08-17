using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class QuestionViewModel
    {
        public QuestionViewModel(JobBasic job, Question question, bool isEditable = false)
        {
            Job = job;
            Question = question;
            IsEditable = isEditable;
        }

        public JobBasic Job { get; set; }
        public Question Question { get; set; }
        public bool IsEditable { get; set; }
    }
}
