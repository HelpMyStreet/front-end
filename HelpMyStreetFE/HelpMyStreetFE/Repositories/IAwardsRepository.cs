using System;
using HelpMyStreetFE.Models.Awards;
using System.Threading.Tasks;
using System.Collections.Generic;
namespace HelpMyStreetFE.Repositories
{
    public interface IAwardsRepository
    {
        Task<List<AwardsModel>> GetAwards();
        Task<CurrentAwardModel> GetAwardsByUserID(int userID, System.Threading.CancellationToken cancellationToken);
    }
}
