using NuLogicEHR.Models;

namespace NuLogicEHR.Repository
{
    public interface ISettingRepository
    {
        Task<int> CreateAsync(Provider provider);
        Task<Provider?> GetByIdAsync(int id);
        Task<IEnumerable<Provider>> GetAllAsync();
    }
}
