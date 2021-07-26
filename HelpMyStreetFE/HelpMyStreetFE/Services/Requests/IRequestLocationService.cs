using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreetFE.Models.Account.Jobs;

namespace HelpMyStreetFE.Services.Requests
{
    public interface IRequestLocationService
    {
        Task<JobLocation> LocateJob(int jobId, int userId, CancellationToken cancellationToken);
        Task<JobLocation> LocateRequest(int requestId, int userId, CancellationToken cancellationToken);
    }
}
