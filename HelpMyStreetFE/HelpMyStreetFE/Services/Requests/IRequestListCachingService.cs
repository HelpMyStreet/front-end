using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IRequestListCachingService
    {
        Task<IEnumerable<int>> GetGroupRequestsAsync(int groupId, CancellationToken cancellationToken);
        Task<IEnumerable<int>> GetUserRequestsAsync(int userId, CancellationToken cancellationToken);
        Task<IEnumerable<int>> GetUserOpenJobsAsync(User user, CancellationToken cancellationToken);

        Task RefreshGroupRequestsCacheAsync(int groupId, CancellationToken cancellationToken);
        Task RefreshUserRequestsCacheAsync(int userId, CancellationToken cancellationToken);
        Task RefreshUserOpenJobsCacheAsync(User user, CancellationToken cancellationToken);
    }
}
