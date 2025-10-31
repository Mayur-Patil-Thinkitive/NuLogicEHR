using Microsoft.EntityFrameworkCore;
using NuLogicEHR.Configurations;
using NuLogicEHR.Models;

namespace NuLogicEHR.Repository
{
    public class IntakeFormRepository : IIntakeFormRepository
    {
        private readonly ApplicationDbContext _context;

        public IntakeFormRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<int> CreateHistoryAsync(PatientIntakeHistory history)
        {
            _context.PatientIntakeHistories.Add(history);
            await _context.SaveChangesAsync();
            return history.Id;
        }

        public async Task<PatientIntakeHistory?> GetHistoryByIdAsync(int id)
        {
            return await _context.PatientIntakeHistories
                .Include(h => h.PatientDemographic)
                .FirstOrDefaultAsync(h => h.Id == id);
        }
        public async Task UpdateHistoryAsync(PatientIntakeHistory history)
        {
            _context.PatientIntakeHistories.Update(history);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteHistoryAsync(int id)
        {
            var history = await _context.PatientIntakeHistories.FindAsync(id);
            if (history != null)
            {
                _context.PatientIntakeHistories.Remove(history);
                await _context.SaveChangesAsync();
            }
        }
    }
}