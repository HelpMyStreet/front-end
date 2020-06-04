using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class TasksViewModel
    {
        public int ID { get; set; }
        public SupportActivities SupportActivity { get; set; }
        public List<RequestHelpQuestion> Questions { get; set; }
        public bool IsSelected { get; set; }        
    }
}
