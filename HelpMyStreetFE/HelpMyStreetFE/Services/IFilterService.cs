using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models.Account.Jobs;

namespace HelpMyStreetFE.Services
{
    public interface IFilterService
    {
        public SortAndFilterSet GetDefaultSortAndFilterSet(JobSet jobSet, User user);
    }
}
