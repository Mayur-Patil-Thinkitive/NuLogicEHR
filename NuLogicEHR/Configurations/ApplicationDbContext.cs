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
        public DbSet<PatientDemographic> PatientDemographics { get; set; }
        public DbSet<PatientContactInformation> PatientContactInformation { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<InsuranceInformation> InsuranceInformation { get; set; }
        public DbSet<OtherInformation> OtherInformation { get; set; }
        public DbSet<SchedulingAppointment> SchedulingAppointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(_schema);
            
            // Tenant configuration (only for public schema)
            if (_schema == "public")
            {
                modelBuilder.Entity<Tenant>().HasIndex(t => t.HospitalName).IsUnique();
            }

            // Patient relationships (for tenant schemas)
            modelBuilder.Entity<PatientContactInformation>()
                .HasOne(p => p.PatientDemographic)
                .WithOne()
                .HasForeignKey<PatientContactInformation>(p => p.PatientId);

            modelBuilder.Entity<EmergencyContact>()
                .HasOne(e => e.PatientDemographic)
                .WithMany()
                .HasForeignKey(e => e.PatientId);

            modelBuilder.Entity<InsuranceInformation>()
                .HasOne(i => i.PatientDemographic)
                .WithOne()
                .HasForeignKey<InsuranceInformation>(i => i.PatientId);

            modelBuilder.Entity<OtherInformation>()
                .HasOne(o => o.PatientDemographic)
                .WithOne()
                .HasForeignKey<OtherInformation>(o => o.PatientId);

            modelBuilder.Entity<SchedulingAppointment>()
                .HasOne(s => s.PatientDemographic)
                .WithMany()
                .HasForeignKey(s => s.PatientId);
        }
    }
}
