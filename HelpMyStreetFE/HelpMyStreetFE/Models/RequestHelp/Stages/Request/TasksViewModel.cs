using HelpMyStreet.Utils.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class TasksViewModel
    {
        public SupportActivities SupportActivity { get; set; }
        public bool IsSelected { get; set; }        
    }
}
