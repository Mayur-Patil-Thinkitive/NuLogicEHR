using NuLogicEHR.Models;

namespace NuLogicEHR.Repository
{
    public interface IIntakeFormRepository
    {
        Task<int> CreateHistoryAsync(PatientIntakeHistory history);
        Task<PatientIntakeHistory?> GetHistoryByIdAsync(int id);
        Task UpdateHistoryAsync(PatientIntakeHistory history);
        Task DeleteHistoryAsync(int id);
    }
}