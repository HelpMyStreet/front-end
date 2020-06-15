using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.RequestHelp
{
    public class SuccessViewModel
    {
        public List<NotificationModel> Notifications { get; set; }
        public string RequestLink { get; set; }

    }   

}
