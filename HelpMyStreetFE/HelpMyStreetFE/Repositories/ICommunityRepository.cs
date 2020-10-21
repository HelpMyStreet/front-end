using HelpMyStreetFE.Models.Community;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public interface ICommunityRepository
    {
        Task<CommunityViewModel> GetCommunity(string communityName, CancellationToken cancellationToken);
        Task<List<CommunityModel>> GetCommunities();
    }
}