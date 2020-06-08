using HelpMyStreetFE.Models.Community;

namespace HelpMyStreetFE.Repositories
{
    public interface ICommunityRepository
    {
        CommunityViewModel GetCommunity(string communityName);
    }
}