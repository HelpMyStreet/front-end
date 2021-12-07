using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreetFE.Models;

namespace HelpMyStreetFE.Services
{
    public interface IPartnerService
    {
        Task<IEnumerable<Partner>> GetPartners();
    }
}
