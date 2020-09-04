using HelpMyStreetFE.Models.Community;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Repositories
{
    public interface ICommunityRepository
    {
        Task<CommunityViewModel> GetCommunity(string communityName, CancellationToken cancellationToken);
    }
}