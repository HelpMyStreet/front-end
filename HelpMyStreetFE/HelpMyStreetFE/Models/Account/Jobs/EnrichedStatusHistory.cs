using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelpMyStreet.Contracts.RequestService.Response;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class EnrichedStatusHistory
    {
        public EnrichedStatusHistory(StatusHistory statusHistory)
        {
            this.StatusHistory = statusHistory;
        }

        public StatusHistory StatusHistory { get; set; }
        public string JobStatusDescription { get; set; }
        public User VolunteerUser { get; set; }
        public User ChangeMadeByUser { get; set; }
    }
}
