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
            var result = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetGroupRequestsFromRepo(groupId);
            }, GetGroupRequestsCacheKey(groupId), RefreshBehaviour.DontWaitForFreshData, cancellationToken, GetNotInCacheBehaviour(waitForData));

            if (result == default && waitForData)
            {
                throw new Exception($"Unable to get group requests for group {groupId}");
            }
            return result;
        }

        public async Task<IEnumerable<int>> GetUserOpenJobsAsync(User user, bool waitForData, CancellationToken cancellationToken)
        {
            var result = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetUserOpenJobsFromRepo(user);
            }, GetUserOpenJobsCacheKey(user.ID), RefreshBehaviour.DontWaitForFreshData, cancellationToken, GetNotInCacheBehaviour(waitForData), ResetTimeFactory.OnMinute);

            if (result == default && waitForData)
            {
                throw new Exception($"Unable to get user open jobs for user {user.ID}");
            }
            return result;
        }

        public async Task<IEnumerable<int>> GetUserRequestsAsync(int userId, bool waitForData, CancellationToken cancellationToken)
        {
            var result = await _memDistCache.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await GetUserRequestsFromRepo(userId);
            }, GetUserRequestsCacheKey(userId), RefreshBehaviour.DontWaitForFreshData, cancellationToken, GetNotInCacheBehaviour(waitForData));

            if (result == default && waitForData)
            {
                throw new Exception($"Unable to get user requests for user {userId}");
            }
            return result;
        }

        public async Task RefreshGroupRequestsCacheAsync(int groupId, CancellationToken cancellationToken)
        {
            await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
            {
                return await GetGroupRequestsFromRepo(groupId);
            }, GetGroupRequestsCacheKey(groupId), cancellationToken);
        }

        public async Task RefreshUserOpenJobsCacheAsync(User user, CancellationToken cancellationToken)
        {
            await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
            {
                return await GetUserOpenJobsFromRepo(user);
            }, GetUserOpenJobsCacheKey(user.ID), cancellationToken, ResetTimeFactory.OnMinute);
        }

        public async Task RefreshUserRequestsCacheAsync(int userId, CancellationToken cancellationToken)
        {
            {
                await _memDistCache.RefreshDataAsync(async (cancellationToken) =>
                {
                    return await GetUserRequestsFromRepo(userId);
                }, GetUserRequestsCacheKey(userId), cancellationToken);
            }
        }

        private async Task<IEnumerable<int>> GetGroupRequestsFromRepo(int groupId)
        {
            return await _requestHelpRepository.GetRequestIDsForGroup(new GetRequestIDsForGroupRequest
            {
                GroupID = groupId,
                IncludeChildGroups = true,
            });
        }

        private async Task<IEnumerable<int>> GetUserOpenJobsFromRepo(User user)
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

        private async Task<IEnumerable<int>> GetUserRequestsFromRepo(int userId)
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
