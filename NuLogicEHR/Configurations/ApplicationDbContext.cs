using Microsoft.EntityFrameworkCore;
using NuLogicEHR.Models;

namespace NuLogicEHR.Configurations
{
    public class ApplicationDbContext : DbContext
    {
        private readonly string _schema;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, string schema = "public") : base(options)
        {
            _schema = schema;
        }

        public string Schema => _schema;
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Patient> Patients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_schema);
            modelBuilder.Entity<Tenant>().HasIndex(t => t.HospitalName).IsUnique();
            // No Patient-Tenant relationship needed - schema isolation handles tenancy
        }
    }
}
