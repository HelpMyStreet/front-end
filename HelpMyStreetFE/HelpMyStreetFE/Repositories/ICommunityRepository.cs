﻿using HelpMyStreetFE.Models.Community;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Repositories
{
    public interface ICommunityRepository
    {
        Task<CommunityViewModel> GetCommunity(string communityName, CancellationToken cancellationToken);
        Task<CommunityViewModel> GetCommunity(int groupId, CancellationToken cancellationToken);
        Task<CommunityViewModel> GetCommunity(Group group, CancellationToken cancellationToken);
        Task<List<CommunityModel>> GetCommunities();
        CommunityModel GetCommunityDetailByKey(string key);
 }
}