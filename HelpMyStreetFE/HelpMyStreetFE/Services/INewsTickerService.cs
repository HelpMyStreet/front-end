using System.Collections.Generic;
using System.Threading.Tasks;
using HelpMyStreet.Contracts;

namespace HelpMyStreetFE.Services
{
    public interface INewsTickersService
    {
        Task<List<NewsTickerMessage>> GetNewsTickerMessages(int? groupId);
    }
}
