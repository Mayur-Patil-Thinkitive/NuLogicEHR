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
        public async Task DeletePatientAsync(int patientId)
        {
            var patient = await _context.PatientDemographics.FindAsync(patientId);
            if (patient == null)
                throw new InvalidOperationException($"Patient with ID {patientId} not found");

            _context.PatientDemographics.Remove(patient);
            await _context.SaveChangesAsync();
        }
    }
}