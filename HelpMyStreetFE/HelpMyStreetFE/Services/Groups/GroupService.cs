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
using HelpMyStreetFE.Models.RequestHelp;
using HelpMyStreet.Contracts;
using HelpMyStreet.Contracts.ReportService;

namespace HelpMyStreetFE.Services.Groups
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMemDistCache<int> _memDistCache_int;
        private readonly IMemDistCache<Group> _memDistCache_group;
        private readonly IMemDistCache<List<Group>> _memDistCache_groups;
        private readonly IMemDistCache<List<List<GroupCredential>>> _memDistCache_listListGroupCred;
        private readonly IMemDistCache<Instructions> _memDistCache_instructions;

        private const string CACHE_KEY_PREFIX = "group-service-";

        public GroupService(IGroupRepository groupRepository,
            IMemDistCache<int> memDistCache_int,
            IMemDistCache<Group> memDistCache_group,
            IMemDistCache<List<Group>> memDistCache_groups,
            IMemDistCache<List<List<GroupCredential>>> memDistCache_listListGroupCred,
            IMemDistCache<Instructions> memDistCache_instructions)
        {
            _groupRepository = groupRepository;
            _memDistCache_int = memDistCache_int;
            _memDistCache_group = memDistCache_group;
            _memDistCache_groups = memDistCache_groups;
            _memDistCache_listListGroupCred = memDistCache_listListGroupCred;
            _memDistCache_instructions = memDistCache_instructions;
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

        public async Task<RegistrationFormVariant> GetRegistrationFormVariant(int groupId, string source, CancellationToken cancellationToken)
        {
            int registrationFormVariant = await _memDistCache_int.GetCachedDataAsync(async (cancellationToken) =>
            {
                var groupServiceResponse = await _groupRepository.GetRegistrationFormVariant(groupId, source);

                return (int)(groupServiceResponse?.RegistrationFormVariant ?? RegistrationFormVariant.Default);
            }, $"{CACHE_KEY_PREFIX}-registration-form-variant-{groupId}-{source}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);

            return (RegistrationFormVariant)registrationFormVariant;
        }

        public async Task<List<Location>> GetUserLocations(int userId)
        {
            return await _groupRepository.GetUserLocations(userId);
        }

        public async Task<RequestHelpJourney> GetRequestHelpFormVariant(int groupId, string source)
        {
            var groupServiceResponse = await _groupRepository.GetRequestHelpFormVariant(groupId, source);

            if (groupServiceResponse == null)
            {
                throw new Exception($"Could not find RequestHelpFormVariant for group {groupId} and source {source}");
            }

            return new RequestHelpJourney()
            {
                AccessRestrictedByRole = groupServiceResponse.AccessRestrictedByRole,
                RequestHelpFormVariant = groupServiceResponse.RequestHelpFormVariant,
                RequestorDefinedByGroup = groupServiceResponse.RequestorDefinedByGroup
            };
        }


        public async Task<Group> GetGroupById(int groupId, CancellationToken cancellationToken)
        {
            return await _memDistCache_group.GetCachedDataAsync(async (cancellationToken) =>
            {
                return (await _groupRepository.GetGroup(groupId)).Group;
            }, $"{CACHE_KEY_PREFIX}-group-{groupId}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<Group> GetGroupByKey(string groupKey, CancellationToken cancellationToken)
        {
            int groupId = await GetGroupIdByKey(groupKey, cancellationToken);
            return await GetGroupById(groupId, cancellationToken);
        }

        public async Task<List<List<GroupCredential>>> GetGroupActivityCredentials(int groupId, SupportActivities supportActivity, CancellationToken cancellationToken)
        {
            var result = await _memDistCache_listListGroupCred.GetCachedDataAsync(async (cancellationToken) =>
            {
                var credentialSetsWithIds = await _groupRepository.GetGroupActivityCredentials(groupId, supportActivity);
                var groupCredentials = await _groupRepository.GetGroupCredentials(groupId);
                return credentialSetsWithIds.Select(cs => cs.Select(credentialId => groupCredentials.First(gc => gc.CredentialID == credentialId)).ToList()).ToList();
            }, $"{CACHE_KEY_PREFIX}-group-activity-credentials-group-{groupId}-activity-{supportActivity}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);

            if (result == null)
            {
                throw new Exception($"Exception in GetGroupActivityCredentials for group {groupId} and activity {supportActivity}");
            }

            return result;
        }

        public async Task<List<GroupCredential>> GetGroupCredentials(int groupId)
        {
            var groupCredentials = await _groupRepository.GetGroupCredentials(groupId);

            if (groupCredentials == null)
            {
                throw new Exception($"Unable to get group credentials for group {groupId}");
            }

            return groupCredentials;
        }

        public async Task<GroupCredential> GetGroupCredential(int groupId, int credentialId)
        {
            var credential = (await GetGroupCredentials(groupId)).FirstOrDefault(c => c.CredentialID == credentialId);

            if (credential == null)
            {
                throw new Exception($"Unable to find credential {credentialId} for group {groupId}");
            }

            return credential;
        }

        public async Task<Instructions> GetGroupSupportActivityInstructions(int groupId, SupportActivities supportActivity, CancellationToken cancellationToken)
        {
            var instructions = await _memDistCache_instructions.GetCachedDataAsync(async (cancellationToken) =>
            {
                return await _groupRepository.GetGroupSupportActivityInstructions(groupId, supportActivity);
            }, $"{CACHE_KEY_PREFIX}-group-support-activity-instructions-group-{groupId}-activity-{supportActivity}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);

            if (instructions == null)
            {
                throw new Exception($"Unable to find instructions for group {groupId} and activity {supportActivity}");
            }

            return instructions;
        }

        public async Task<Dictionary<SupportActivities, Instructions>> GetAllGroupSupportActivityInstructions(int groupId, IEnumerable<SupportActivities> supportActivities, CancellationToken cancellationToken)
        {
            var keyValuePairs = await Task.WhenAll(
                supportActivities.Select(async sa => 
                    new KeyValuePair<SupportActivities, Instructions>(sa, await GetGroupSupportActivityInstructions(groupId, sa, cancellationToken))
                )
            );

            return keyValuePairs.ToDictionary(x => x.Key, x => x.Value);
        }

        public async Task<List<Group>> GetChildGroups(int groupId)
        {
            return (await _groupRepository.GetChildGroups(groupId)).ChildGroups;
        }

        public async Task<List<SupportActivityDetail>> GetSupportActivitesForRegistrationForm(RegistrationFormVariant registrationFormVariant)
        {
            var response = await _groupRepository.GetRegistrationFormSupportActivies(registrationFormVariant);
            if (response.SupportActivityDetails.Count() == 0)
            {
                throw new Exception($"No support activies for registration form: {registrationFormVariant}");
            }
            return response.SupportActivityDetails;
        }


        public async Task<List<Group>> GetGroupsWithMapDetails(MapLocation mapLocation, CancellationToken cancellationToken)
        {
            return await _memDistCache_groups.GetCachedDataAsync(async (cancellationToken) =>
            {
                return (await _groupRepository.GetGroupsWithMapDetails(mapLocation)).Groups;
            }, $"{CACHE_KEY_PREFIX}-group-maps-{(int)mapLocation}", RefreshBehaviour.DontWaitForFreshData, cancellationToken);
        }

        public async Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId)
        {
            return await _groupRepository.GetNewsTickerMessages(groupId);
        }

        public async Task<Chart> GetChart(Charts chart, int groupId)
        {
            return await _groupRepository.GetChart(chart, groupId);
        }
    }
}