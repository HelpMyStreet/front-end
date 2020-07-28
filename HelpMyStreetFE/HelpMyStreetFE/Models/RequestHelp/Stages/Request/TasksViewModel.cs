using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class TasksViewModel
    {
        public TasksViewModel()
        {
            Questions = new List<RequestHelpQuestion>();
        }

        public SupportActivities SupportActivity { get; set; }
        public List<RequestHelpQuestion> Questions { get; set; }
        public bool IsSelected { get; set; }        
    }
}
