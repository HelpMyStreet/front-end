using System.Threading.Tasks;
using HelpMyStreetFE.Repositories;
using System.Collections.Generic;
using System.Linq;
using HelpMyStreet.Utils.Enums;
using System;
using HelpMyStreet.Cache;
using System.Threading;
using HelpMyStreet.Utils.Models;
using HelpMyStreet.Contracts.GroupService.Response;

namespace HelpMyStreetFE.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMemDistCache<int> _memDistCache_int;
        private readonly IMemDistCache<Group> _memDistCache_group;
        private readonly IMemDistCache<List<List<GroupCredential>>> _memDistCache_listListGroupCred;

        private const string CACHE_KEY_PREFIX = "group-service-";

        public GroupService(IGroupRepository groupRepository, IMemDistCache<int> memDistCache_int, IMemDistCache<Group> memDistCache_group, IMemDistCache<List<List<GroupCredential>>> memDistCache_listListGroupCred)
        {
            _groupRepository = groupRepository;
            _memDistCache_int = memDistCache_int;
            _memDistCache_group = memDistCache_group;
            _memDistCache_listListGroupCred = memDistCache_listListGroupCred;
        }

        public async Task<int> GetGroupIdByKey(string groupKey, CancellationToken cancellationToken)
        {
            return await _memDistCache_int.GetCachedDataAsync(async (cancellationToken) =>
            {
                var groupServiceResponse = await _groupRepository.GetGroupByKey(groupKey);

                if (groupServiceResponse == null) { throw new Exception($"Could not identify group for key '{groupKey}'"); }

                return groupServiceResponse.GroupId;
            }, $"{CACHE_KEY_PREFIX}-group-id-{groupKey}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<RegistrationFormVariant?> GetRegistrationFormVariant(int groupId, string source)
        {
            var groupServiceResponse = await _groupRepository.GetRegistrationFormVariant(groupId, source);

            return groupServiceResponse?.RegistrationFormVariant;
        }

        public async Task<RequestHelpFormVariant> GetRequestHelpFormVariant(int groupId, string source)
        {
            var groupServiceResponse = await _groupRepository.GetRequestHelpFormVariant(groupId, source);

            if (groupServiceResponse == null)
            {
                throw new Exception($"Could not find RequestHelpFormVariant for group {groupId} and source {source}");
            }

            return groupServiceResponse.RequestHelpFormVariant;
        }


        public async Task<Group> GetGroupById(int groupId, CancellationToken cancellationToken)
        {
            return await _memDistCache_group.GetCachedDataAsync(async (cancellationToken) =>
            {
                return (await _groupRepository.GetGroup(groupId)).Group;
            }, $"{CACHE_KEY_PREFIX}-group-{groupId}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);

        }

        public async Task<List<List<GroupCredential>>> GetGroupActivityCredentials(int groupId, SupportActivities supportActivitiy, CancellationToken cancellationToken)
        {
            var result = await _memDistCache_listListGroupCred.GetCachedDataAsync(async (cancellationToken) =>
            {
                var credentialSetsWithIds = await _groupRepository.GetGroupActivityCredentials(groupId, supportActivitiy);
                var groupCredentials = await _groupRepository.GetGroupCredentials(groupId);
                return credentialSetsWithIds.Select(cs => cs.Select(credentialId => groupCredentials.First(gc => gc.CredentialID == credentialId)).ToList()).ToList();
            }, $"{CACHE_KEY_PREFIX}-group-activity-credentials-group-{groupId}-activity-{supportActivitiy}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);

            if (result == null)
            {
                throw new Exception($"Exception in GetGroupActivityCredentials for group {groupId} and activity {supportActivitiy}");
            }

            return result;
        }

        public async Task<List<GroupCredential>> GetGroupCredentials (int groupId)
        {
            return await _groupRepository.GetGroupCredentials(groupId);
        }
    }
}