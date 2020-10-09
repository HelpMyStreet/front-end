using System.Threading.Tasks;
using HelpMyStreetFE.Repositories;
using System.Collections.Generic;
using System.Linq;
using HelpMyStreet.Utils.Enums;
using System;
using HelpMyStreet.Cache;
using System.Threading;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMemDistCache<int> _memDistCache_int;
        private readonly IMemDistCache<Group> _memDistCache_group;

        private const string CACHE_KEY_PREFIX = "group-service-";

        public GroupService(IGroupRepository groupRepository, IMemDistCache<int> memDistCache_int, IMemDistCache<Group> memDistCache_group)
        {
            _groupRepository = groupRepository;
            _memDistCache_int = memDistCache_int;
            _memDistCache_group = memDistCache_group;
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
    }
}