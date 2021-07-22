using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Cache;
using HelpMyStreet.Contracts.RequestService.Request;
using HelpMyStreet.Utils.Enums;
using HelpMyStreet.Utils.Models;
using HelpMyStreetFE.Repositories;
using HelpMyStreetFE.Services.Groups;
using Microsoft.Extensions.Logging;

namespace HelpMyStreetFE.Services.Requests
{
    public class RequestListCachingService : IRequestListCachingService
    {
        private readonly IMemDistCache<IEnumerable<int>> _memDistCache;
        private readonly IRequestHelpRepository _requestHelpRepository;
        private readonly IGroupMemberService _groupMemberService;
        private readonly ILogger<RequestListCachingService> _logger;

        private const string CACHE_KEY_PREFIX = "request-list-caching-service";

        public RequestListCachingService(
            IMemDistCache<IEnumerable<int>> memDistCache, 
            IRequestHelpRepository requestHelpRepository, 
            IGroupMemberService groupMemberService,
            ILogger<RequestListCachingService> logger)
        {
            _memDistCache = memDistCache ?? throw new ArgumentNullException(nameof(memDistCache));
            _requestHelpRepository = requestHelpRepository ?? throw new ArgumentNullException(nameof(requestHelpRepository));
            _groupMemberService = groupMemberService ?? throw new ArgumentNullException(nameof(groupMemberService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<int>> GetGroupRequestsAsync(int groupId, bool waitForData, CancellationToken cancellationToken)
        {
            return await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetGroupRequestsFromRepo(groupId, cancellationToken);
            }, GetGroupRequestsCacheKey(groupId), RefreshBehaviour.DontWaitForFreshData, cancellationToken, GetNotInCacheBehaviour(waitForData));
        }

        public async Task<IEnumerable<int>> GetUserOpenJobsAsync(User user, bool waitForData, CancellationToken cancellationToken)
        {
            return await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetUserOpenJobsFromRepo(user, cancellationToken);
            }, GetUserOpenJobsCacheKey(user.ID), RefreshBehaviour.DontWaitForFreshData, cancellationToken, GetNotInCacheBehaviour(waitForData));
        }

        public async Task<IEnumerable<int>> GetUserRequestsAsync(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            return await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetUserRequestsFromRepo(userId, cancellationToken);
            }, GetUserRequestsCacheKey(userId), RefreshBehaviour.DontWaitForFreshData, cancellationToken, GetNotInCacheBehaviour(waitForData));
        }

        public async Task RefreshGroupRequestsCacheAsync(int groupId, CancellationToken cancellationToken)
        {
            await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
            {
                return await GetGroupRequestsFromRepo(groupId, cancellationToken);
            }, GetGroupRequestsCacheKey(groupId), cancellationToken);
        }

        public async Task RefreshUserOpenJobsCacheAsync(User user, CancellationToken cancellationToken)
        {
            await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
            {
                return await GetUserOpenJobsFromRepo(user, cancellationToken);
            }, GetUserOpenJobsCacheKey(user.ID), cancellationToken);
        }

        public async Task RefreshUserRequestsCacheAsync(int userId, CancellationToken cancellationToken)
        {
            {
                await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetUserRequestsFromRepo(userId, cancellationToken);
                }, GetUserRequestsCacheKey(userId), cancellationToken);
            }
        }

        private async Task<IEnumerable<int>> GetGroupRequestsFromRepo(int groupId, CancellationToken cancellationToken)
        {
            var requests = await _requestHelpRepository.GetRequestsByFilter(new GetRequestsByFilterRequest()
            {
                ReferringGroupID = groupId,
                IncludeChildGroups = true,
            });
            return requests.Select(r => r.RequestID);
        }

        private async Task<IEnumerable<int>> GetUserOpenJobsFromRepo(User user, CancellationToken cancellationToken)
        {
            if (user.PostalCode == null)
            {
                throw new Exception("Cannot get open jobs for user without postcode");
            }
            var jobsByFilterRequest = new GetAllJobsByFilterRequest()
            {
                Postcode = user.PostalCode,
                JobStatuses = new JobStatusRequest()
                {
                    JobStatuses = new List<JobStatuses>() { JobStatuses.Open }
                },
                Groups = new GroupRequest() { Groups = await _groupMemberService.GetUserGroups(user.ID) },
            };
            var jobs = await _requestHelpRepository.GetAllJobsByFilterAsync(jobsByFilterRequest);
            var jobIDs = jobs.JobBasics.Select(j => j.JobID);
            return jobIDs;
        }

        private async Task<IEnumerable<int>> GetUserRequestsFromRepo(int userId, CancellationToken cancellationToken)
        {
            var request = new GetAllJobsByFilterRequest { AllocatedToUserId = userId };
            var jobs = await _requestHelpRepository.GetAllJobsByFilterAsync(request);
            var requestIDs = jobs.JobBasics .Select(j => j.RequestID).Distinct();
            return requestIDs;
        }

        private string GetGroupRequestsCacheKey(int groupId)
        {
            return $"{CACHE_KEY_PREFIX}-group-{groupId}-requests";
        }

        private string GetUserOpenJobsCacheKey(int userId)
        {
            return $"{CACHE_KEY_PREFIX}-user-{userId}-open-jobs";
        }

        private string GetUserRequestsCacheKey(int userId)
        {
            return $"{CACHE_KEY_PREFIX}-user-{userId}-requests";
        }

        private NotInCacheBehaviour GetNotInCacheBehaviour(bool waitForData)
        {
            return waitForData ? NotInCacheBehaviour.WaitForData : NotInCacheBehaviour.DontWaitForData;
        }
    }
}
