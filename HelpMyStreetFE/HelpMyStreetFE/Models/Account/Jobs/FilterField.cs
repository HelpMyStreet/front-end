using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class FilterField<T>
    {
        public T Value { get; set; }
        public bool IsSelected { get; set; }
        public int MatchingItems { get; set; }
    }
}
