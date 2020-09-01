using HelpMyStreetFE.Enums.Account;
using System;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class JobFilterViewModel
    {
        public FilterSet FilterSet { get; set; }
        public JobFilterRequest JobFilterRequest { get; set; }
        public Action EmptyJobSetCallback { get; set; }
    }
}
