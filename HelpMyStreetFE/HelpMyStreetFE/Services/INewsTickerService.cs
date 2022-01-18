using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HelpMyStreet.Contracts;
using HelpMyStreet.Utils.Models;

namespace HelpMyStreetFE.Services
{
    public interface INewsTickersService
    {
        Task<IEnumerable<NewsTickerMessage>> GetNewsTickerMessages(int? groupId, CancellationToken cancellationToken);
    }
}
