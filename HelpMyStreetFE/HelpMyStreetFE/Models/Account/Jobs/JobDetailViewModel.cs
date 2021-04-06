using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobDetailViewModel
    {
        public JobViewModel<JobDetail> JobDetail { get; set; }
        public Instructions GroupSupportActivityInstructions { get; set; }
        public bool ToPrint { get; set; } = false;
    }
}
