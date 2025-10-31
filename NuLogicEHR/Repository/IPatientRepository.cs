using NuLogicEHR.Models;

namespace NuLogicEHR.Repository
{
    public interface IPatientRepository
    {
        Task<int> AddPatientDemographicAsync(PatientDemographic demographic);
        Task<int> AddPatientContactAsync(PatientContactInformation contact);
        Task<int> AddEmergencyContactAsync(EmergencyContact emergency);
        Task<int> AddInsuranceInformationAsync(InsuranceInformation insurance);
        Task<int> AddOtherInformationAsync(OtherInformation other);
        Task DeletePatientAsync(int patientId);
    }
}