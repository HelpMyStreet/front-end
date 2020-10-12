using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using System;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobFilterViewModel
    {
        public string PanelId { get; set; }
        public SortAndFilterSet FilterSet { get; set; }
        public JobFilterRequest JobFilterRequest { get; set; }
        public Action EmptyJobSetCallback { get; set; }
        public User User { get; set; }
    }
}
