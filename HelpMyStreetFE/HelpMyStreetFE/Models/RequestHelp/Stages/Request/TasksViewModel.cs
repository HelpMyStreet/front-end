using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class TasksViewModel : BasicTileViewModel
    {
        public TasksViewModel()
        {
            DataType = "activities";
        }

        public SupportActivities SupportActivity { get; set; }

        public int ID
        {
            get
            {
                return (int)SupportActivity;
            }
        }

        public override string Value
        {
            get
            {
                return SupportActivity.ToString();
            }
            set { }
        }

        public string Description
        {
            get
            {
                return SupportActivity.FriendlyNameShort();
            }
        }
    }
}
