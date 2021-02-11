using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Models.Account.Jobs;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IListViewBuilder<T> where T : JobBasic
    {
        Task<ListViewModel<JobViewModel<T>>> BuildList(User user, JobFilterRequest jobFilterRequest, Action hideFilterPanelCallback, Action noJobsCallback, CancellationToken cancellationToken);
    }
}
