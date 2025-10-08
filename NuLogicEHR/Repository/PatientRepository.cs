using Microsoft.EntityFrameworkCore;
using NuLogicEHR.Configurations;
using NuLogicEHR.Models;

namespace NuLogicEHR.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;
        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddPatientDemographicAsync(PatientDemographic demographic)
        {
            _context.PatientDemographics.Add(demographic);
            await _context.SaveChangesAsync();
            return demographic.Id;
        }

        public async Task<int> AddPatientContactAsync(PatientContactInformation contact)
        {
            _context.PatientContactInformation.Add(contact);
            await _context.SaveChangesAsync();
            return contact.Id;
        }

        public async Task<int> AddEmergencyContactAsync(EmergencyContact emergency)
        {
            _context.EmergencyContacts.Add(emergency);
            await _context.SaveChangesAsync();
            return emergency.Id;
        }

        public async Task<int> AddInsuranceInformationAsync(InsuranceInformation insurance)
        {
            _context.InsuranceInformation.Add(insurance);
            await _context.SaveChangesAsync();
            return insurance.Id;
        }

        public async Task<int> AddOtherInformationAsync(OtherInformation other)
        {
            _context.OtherInformation.Add(other);
            await _context.SaveChangesAsync();
            return other.Id;
        }

        public async Task<object> GetPatientByIdAsync(int patientId)
        {
            var demographic = await _context.PatientDemographics.FirstOrDefaultAsync(p => p.Id == patientId);
            if (demographic == null) throw new InvalidOperationException("Patient not found");

            var contact = await _context.PatientContactInformation.FirstOrDefaultAsync(c => c.PatientId == patientId);
            var emergencyContacts = await _context.EmergencyContacts.Where(e => e.PatientId == patientId).ToListAsync();
            var insurance = await _context.InsuranceInformation.FirstOrDefaultAsync(i => i.PatientId == patientId);
            var otherInfo = await _context.OtherInformation.FirstOrDefaultAsync(o => o.PatientId == patientId);

            return new
            {
                Demographic = demographic,
                Contact = contact,
                EmergencyContacts = emergencyContacts,
                Insurance = insurance == null ? null : new
                {
                    insurance.Id,
                    PaymentMethod = insurance.PaymentMethod.HasValue ? (insurance.PaymentMethod.Value ? "SelfPay" : "Insurance") : null,
                    insurance.InsuranceType,
                    insurance.InsuranceName,
                    insurance.MemberId,
                    insurance.PlanName,
                    insurance.PlanType,
                    insurance.GroupId,
                    insurance.GroupName,
                    insurance.EffectiveStartDate,
                    insurance.EffectiveEndDate,
                    insurance.PatientRelationshipWithInsured,
                    insurance.PatientId
                },
                OtherInformation = otherInfo
            };
        }
    }
}