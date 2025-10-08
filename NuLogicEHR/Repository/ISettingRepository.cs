using NuLogicEHR.Models;

namespace NuLogicEHR.Repository
{
    public interface ISettingRepository
    {
        Task<int> CreateAsync(Provider provider);
        Task<Provider?> GetByIdAsync(int id);
        Task<IEnumerable<Provider>> GetAllAsync();
        Task<int> CreateStaffAsync(Staff staff);
        Task<IEnumerable<Staff>> GetAllStaffAsync();
        Task<int> CreateSoberLivingHomeAsync(SoberLivingHome soberLivingHome);
        Task<IEnumerable<SoberLivingHome>> GetAllSoberLivingHomesAsync();
    }
}
