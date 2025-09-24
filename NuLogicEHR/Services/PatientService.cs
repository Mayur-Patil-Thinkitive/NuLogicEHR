using Microsoft.EntityFrameworkCore;
using NuLogicEHR.Configurations;
using NuLogicEHR.Models;
using NuLogicEHR.ViewModels;
using NuLogicEHR.Repository;

namespace NuLogicEHR.Services
{
    public class PatientService : BaseService
    {
        public PatientService(TenantService tenantService, DbContextFactory dbContextFactory)
            : base(tenantService, dbContextFactory)
        {
        }

        public async Task<int> CreatePatientDemographicAsync(int tenantId, PatientDemographicViewModel request)
        {
            using var context = await GetContextAsync(tenantId);
            var repository = new PatientRepository(context);
            
            var demographic = new PatientDemographic
            {
                ProfileImagePath = request.ProfileImagePath,
                FirstName = request.FirstName,
                MiddleName = request.MiddleName,
                LastName = request.LastName,
                Suffix = request.Suffix,
                Nickname = request.Nickname,
                GenderAtBirth = request.GenderAtBirth,
                CurrentGender = request.CurrentGender,
                Pronouns = request.Pronouns,
                DateOfBirth = request.DateOfBirth,
                MaritalStatus = request.MaritalStatus,
                TimeZone = request.TimeZone,
                PreferredLanguage = request.PreferredLanguage,
                Occupation = request.Occupation,
                SSN = int.Parse(request.SSN),
                Race = request.Race,
                Ethnicity = request.Ethnicity,
                TreatmentType = request.TreatmentType
            };
            
            return await repository.AddPatientDemographicAsync(demographic);
        }

        public async Task<int> CreatePatientContactAsync(int tenantId, PatientContactViewModel request)
        {
            using var context = await GetContextAsync(tenantId);
            var repository = new PatientRepository(context);
            
            var contact = new PatientContactInformation
            {
                MobileNumber = request.MobileNumber,
                HomeNumber = request.HomeNumber,
                Email = request.Email,
                FaxNumber = request.FaxNumber,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                State = request.State,
                Country = request.Country,
                ZipCode = request.ZipCode,
                PatientId = request.PatientId
            };
            
            return await repository.AddPatientContactAsync(contact);
        }

        public async Task<int> CreateEmergencyContactAsync(int tenantId, EmergencyContactModelViewModel request)
        {
            using var context = await GetContextAsync(tenantId);
            var repository = new PatientRepository(context);
            
            var emergency = new EmergencyContact
            {
                RelationshipWithPatient = request.RelationshipWithPatient,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                PatientId = request.PatientId
            };
            
            return await repository.AddEmergencyContactAsync(emergency);
        }

        public async Task<int> CreateInsuranceInformationAsync(int tenantId, InsuranceInformationViewModel request)
        {
            using var context = await GetContextAsync(tenantId);
            var repository = new PatientRepository(context);
            
            var insurance = new InsuranceInformation
            {
                PaymentMethod = !string.IsNullOrEmpty(request.InsuranceType),
                InsuranceType = request.InsuranceType,
                InsuranceName = request.InsuranceName,
                MemberId = request.MemberId,
                PlanName = request.PlanName,
                PlanType = request.PlanType,
                GroupId = request.GroupId,
                GroupName = request.GroupName,
                EffectiveStartDate = request.EffectiveStartDate,
                EffectiveEndDate = request.EffectiveEndDate,
                PatientRelationshipWithInsured = request.PatientRelationshipWithInsured,
                InsuranceCardFilePath = request.InsuranceCardFilePath,
                PatientId = request.PatientId
            };
            
            return await repository.AddInsuranceInformationAsync(insurance);
        }

        public async Task<int> CreateOtherInformationAsync(int tenantId, OtherInformationViewModel request)
        {
            using var context = await GetContextAsync(tenantId);
            var repository = new PatientRepository(context);
            
            var other = new OtherInformation
            {
                ConsentToEmail = request.ConsentToEmail,
                ConsentToMessage = request.ConsentToMessage,
                PracticeLocation = request.PracticeLocation,
                RegistrationDate = request.RegistrationDate ?? DateTime.UtcNow,
                Source = request.Source,
                PatientId = request.PatientId
            };
            
            return await repository.AddOtherInformationAsync(other);
        }

        public async Task<object> GetPatientByIdAsync(int tenantId, int patientId)
        {
            using var context = await GetContextAsync(tenantId);
            var repository = new PatientRepository(context);
            return await repository.GetPatientByIdAsync(patientId);
        }
    }
}
