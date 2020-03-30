using System;
using System.Threading;
using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public class AddressService : IAddressService
    {
        public int GetVolunteerCount()
        {
            throw new NotImplementedException();
        }

        public Task<int> GetStreetsCovered()
        {
            return Task.Factory.StartNew(() => 1734);
        }

        public Task<int> GetStreetChampions()
        {
            return Task.Factory.StartNew(() => 2834);
        }

        public Task<int> GetStreetsRemaining()
        {
            return Task.Factory.StartNew(() => 8182);
        }

        public Task<int> GetPostCodesCovered()
        {
            return Task.Factory.StartNew(() => 15);
        }
    }
}
