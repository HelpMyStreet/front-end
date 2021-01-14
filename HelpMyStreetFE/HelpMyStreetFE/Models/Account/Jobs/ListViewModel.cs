using System.Collections.Generic;
using System.Linq;

namespace HelpMyStreetFE.Models.Account.Jobs
{
    public class ListViewModel<T>
    {
        public IEnumerable<T> Items { get; set; }
        public int DisplayedItems { get { return Items.Count(); } }
        public int FilteredItems { get; set; }
        public int UnfilteredItems { get; set; }
        public int ResultsToShowIncrement { get; set; }
    }
}
