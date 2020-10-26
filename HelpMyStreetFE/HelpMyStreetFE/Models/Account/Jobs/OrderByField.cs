using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class OrderByField
    {
        public string Label { get; set; }
        public OrderBy Value { get; set; }
        public bool IsSelected { get; set; }
    }
}
