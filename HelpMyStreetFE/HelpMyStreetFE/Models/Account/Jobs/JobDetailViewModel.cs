using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using System.Collections.Generic;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobDetailViewModel
    {
        public bool UserIsVerified { get; set; }
        public JobDetail JobDetail { get; set; }
    }
}
