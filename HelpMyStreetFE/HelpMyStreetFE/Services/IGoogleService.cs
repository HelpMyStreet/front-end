using System.Threading.Tasks;

namespace HelpMyStreetFE.Services
{
    public interface IGoogleService
    {
        Task<string> GetCachedGoogleMapScript();
    }
}