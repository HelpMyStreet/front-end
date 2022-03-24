using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using HelpMyStreetFE.Enums.Account;
using HelpMyStreetFE.Helpers;
using HelpMyStreetFE.Models.Account.Jobs;
using HelpMyStreetFE.Services.Groups;

namespace HelpMyStreetFE.Services.Requests
{
    public class RequestLocationService : IRequestLocationService
    {
        private readonly IRequestCachingService _requestCachingService;
        private readonly IJobCachingService _jobCachingService;
        private readonly IGroupService _groupService;
        private readonly IGroupMemberService _groupMemberService;

        public RequestLocationService(IRequestCachingService requestCachingService, IJobCachingService jobCachingService, IGroupService groupService, IGroupMemberService groupMemberService)
        {
            _requestCachingService = requestCachingService ?? throw new ArgumentNullException(nameof(requestCachingService));
            _jobCachingService = jobCachingService ?? throw new ArgumentNullException(nameof(jobCachingService));
            _groupService = groupService ?? throw new ArgumentNullException(nameof(groupService));
            _groupMemberService = groupMemberService ?? throw new ArgumentNullException(nameof(groupMemberService));
        }

        public async Task<JobLocation> LocateJob(int jobId, int userId, CancellationToken cancellationToken)
        {
            var job = await _jobCachingService.GetJobBasicAsync(jobId, cancellationToken);

            if (job == null)
            {
                throw new Exception($"Failed to locate job {jobId} for user {userId}");
            }

            if (job.VolunteerUserID == userId && job.JobStatus != JobStatuses.Open)
            {
                return new JobLocation
                {
                    JobSet = job.RequestType switch
                    {
                        RequestType.Task => JobSet.UserMyRequests,
                        RequestType.Shift => JobSet.UserMyShifts,
                        _ => throw new ArgumentException($"Unexpected RequestType: {job.RequestType}", nameof(job.RequestType)),
                    }
                };
            }
            else if (await _groupMemberService.GetUserHasRole(userId, job.ReferringGroupID, GroupRoles.TaskAdmin, false, cancellationToken))
            {
                return new JobLocation
                {
                    JobSet = (job.RequestType.Equals(RequestType.Task) ? JobSet.GroupRequests : JobSet.GroupShifts),
                    GroupKey = (await _groupService.GetGroupById(job.ReferringGroupID, cancellationToken)).GroupKey,
                };
            }
            else if (await _groupMemberService.GetUserHasRole(userId, job.ReferringGroupID, GroupRoles.TaskAdmin, true, cancellationToken))
            {
                var group = await _groupService.GetGroupById(job.ReferringGroupID, cancellationToken);
                var parentGroup = await _groupService.GetGroupById(group.ParentGroupId.Value, cancellationToken);
                return new JobLocation
                {
                    JobSet = (job.RequestType.Equals(RequestType.Task) ? JobSet.GroupRequests : JobSet.GroupShifts),
                    GroupKey = parentGroup.GroupKey,
                };
            }
            else if (job.JobStatus == JobStatuses.Open)
            {
                return new JobLocation { JobSet = (job.RequestType.Equals(RequestType.Task) ? JobSet.UserOpenRequests : JobSet.UserOpenShifts) };
            }

            return null;
        }

        public async Task<JobLocation> LocateRequest(int requestId, int userId, CancellationToken cancellationToken)
        {
            var request = await _requestCachingService.GetRequestSummaryAsync(requestId, cancellationToken);

            if (request.JobBasics.Count(j => j.VolunteerUserID == userId && j.JobStatus != JobStatuses.Open) > 0)
            {
                return new JobLocation
                {
                    JobSet = request.RequestType switch
                    {
                        RequestType.Task => JobSet.UserMyRequests,
                        RequestType.Shift => JobSet.UserMyShifts,
                        _ => throw new ArgumentException($"Unexpected RequestType: {request.RequestType}", nameof(request.RequestType)),
                    }
                };
            }
            else if (await _groupMemberService.GetUserHasRole(userId, request.ReferringGroupID, GroupRoles.TaskAdmin, false, cancellationToken))
            {
                return new JobLocation
                {
                    JobSet = (request.RequestType.Equals(RequestType.Task) ? JobSet.GroupRequests : JobSet.GroupShifts),
                    GroupKey = (await _groupService.GetGroupById(request.ReferringGroupID, cancellationToken)).GroupKey,
                };
            }
            else if (await _groupMemberService.GetUserHasRole(userId, request.ReferringGroupID, GroupRoles.TaskAdmin, true, cancellationToken))
            {
                var group = await _groupService.GetGroupById(request.ReferringGroupID, cancellationToken);
                var parentGroup = await _groupService.GetGroupById(group.ParentGroupId.Value, cancellationToken);
                return new JobLocation
                {
                    JobSet = (request.RequestType.Equals(RequestType.Task) ? JobSet.GroupRequests : JobSet.GroupShifts),
                    GroupKey = parentGroup.GroupKey,
                };
            }
            else if (request.JobBasics.JobStatusDictionary().ContainsKey(JobStatuses.Open))
            {
                return new JobLocation { JobSet = (request.RequestType.Equals(RequestType.Task) ? JobSet.UserOpenRequests : JobSet.UserOpenShifts) };
            }

            return null;
        }
    }
}
