using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Models.Account.Jobs;

namespace HelpMyStreetFE.Services
{
    public interface IFilterService
    {
        public FilterSet GetDefaultFilterSet(JobSet jobSet, User user);
    }
}
