using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp.Stages.Request
{
    public class TasksViewModel : IBasicTileViewModel
    {
        public SupportActivities SupportActivity { get; set; }
        public bool IsSelected { get; set; }

        public string DataType
        {
            get
            {
                return "activities";
            }
        }

        public int ID
        {
            get
            {
                return (int)SupportActivity;
            }
        }

        public string Value
        {
            get
            {
                return SupportActivity.ToString();
            }
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
