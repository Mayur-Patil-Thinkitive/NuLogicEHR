using Microsoft.EntityFrameworkCore;
using NuLogicEHR.Configurations;
using NuLogicEHR.Models;

namespace NuLogicEHR.Repository
{
    public class SettingRepository : ISettingRepository
    {
        private readonly ApplicationDbContext _context;

        public SettingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Provider provider)
        {
            _context.Providers.Add(provider);
            await _context.SaveChangesAsync();
            return provider.Id;
        }

        public async Task<Provider?> GetByIdAsync(int id)
        {
            return await _context.Providers.FindAsync(id);
        }

        public async Task<IEnumerable<Provider>> GetAllAsync()
        {
            return await _context.Providers.ToListAsync();
        }

        public async Task<int> CreateStaffAsync(Staff staff)
        {
            _context.Staff.Add(staff);
            await _context.SaveChangesAsync();
            return staff.Id;
        }

        public async Task<IEnumerable<Staff>> GetAllStaffAsync()
        {
            return await _context.Staff.Include(s => s.Credentials).ToListAsync();
        }
        public async Task<int> CreateSoberLivingHomeAsync(SoberLivingHome soberLivingHome)
        {
            _context.SoberLivingHomes.Add(soberLivingHome);
            await _context.SaveChangesAsync();
            return soberLivingHome.Id;
        }

        public async Task<IEnumerable<SoberLivingHome>> GetAllSoberLivingHomesAsync()
        {
            return await _context.SoberLivingHomes.ToListAsync();
        }
    }
}