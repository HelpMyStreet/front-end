using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class QuestionViewModel
    {
        public QuestionViewModel(Question question, bool isEditable = false)
        {
            Question = question;
            IsEditable = isEditable;
        }

        public Question Question { get; set; }
        public bool IsEditable { get; set; }
    }
}
