using HelpMyStreet.Contracts.GroupService.Request;
using HelpMyStreet.Contracts.GroupService.Response;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Enums;
using System.Collections.Generic;
using HelpMyStreet.Utils.Models;
using System;
using HelpMyStreet.Contracts;
using HelpMyStreet.Contracts.ReportService;

namespace HelpMyStreetFE.Repositories
{
    public interface IGroupRepository
    {
        Task<GetGroupByKeyResponse> GetGroupByKey(string groupKey);
        Task<GetGroupResponse> GetGroup(int groupId);
        Task<GetGroupsWithMapDetailsResponse> GetGroupsWithMapDetails(MapLocation mapLocation);
        Task<GetChildGroupsResponse> GetChildGroups(int groupId);
        Task<GetRegistrationFormVariantResponse> GetRegistrationFormVariant(int groupId, string source = "");
        Task<GetRequestHelpFormVariantResponse> GetRequestHelpFormVariant(int groupId, string source = "");
        Task<PostAddUserToDefaultGroupsResponse> PostAddUserToDefaultGroups(int userId);
        Task<GetUserGroupsResponse> GetUserGroups(int userId);
        Task<UserInGroup> GetGroupMember(int groupId, int userId, int authorisingUserId);
        Task<List<UserInGroup>> GetAllGroupMembers(int groupId, int authorisingUserId);
        Task<GetUserRolesResponse> GetUserRoles(int userId);
        Task<GroupPermissionOutcome> PostAssignRole(int userId, int groupId, GroupRoles role, int authorisingUserId);
        Task<GroupPermissionOutcome> PostRevokeRole(int userId, int groupId, GroupRoles role, int authorisingUserId);
        Task<List<List<int>>> GetGroupActivityCredentials(int groupId, SupportActivities supportActivitiy);
        Task<List<GroupCredential>> GetGroupCredentials(int groupId);
        Task<bool> PutGroupMemberCredentials(PutGroupMemberCredentialsRequest putGroupMemberCredentialsRequest);
        Task<Instructions> GetGroupSupportActivityInstructions(int groupId, SupportActivities supportActivity);
        Task<List<Location>> GetGroupLocations(int groupId, bool includeChildGroups);
        Task<List<Location>> GetUserLocations(int userId);
        Task<GetRegistrationFormSupportActivitiesResponse> GetRegistrationFormSupportActivies(RegistrationFormVariant registrationFormVariant);
        Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId);
        Task<Chart> GetChart(Charts chart, int groupId);
    }
}
