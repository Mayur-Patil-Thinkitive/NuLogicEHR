namespace NuLogicEHR.ViewModels
{
    public class PatientCompleteViewModel
    {
        public int PatientId { get; set; }
        public PatientDemographicData? Demographic { get; set; }
        public PatientContactData? Contact { get; set; }
        public List<EmergencyContactData>? EmergencyContacts { get; set; }
        public InsuranceData? Insurance { get; set; }
        public OtherInfoData? OtherInformation { get; set; }
    }

    public class PatientDemographicData
    {
        public string? ProfileImagePath { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Suffix { get; set; }
        public string? Nickname { get; set; }
        public string? GenderAtBirth { get; set; }
        public string? CurrentGender { get; set; }
        public string? Pronouns { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? MaritalStatus { get; set; }
        public string? TimeZone { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? Occupation { get; set; }
        public int? SSN { get; set; }
        public string? SSNNote { get; set; }
        public string? Race { get; set; }
        public string? Ethnicity { get; set; }
        public string? TreatmentType { get; set; }
    }

    public class PatientContactData
    {
        public string? MobileNumber { get; set; }
        public string? HomeNumber { get; set; }
        public string? Email { get; set; }
        public string? EmailNote { get; set; }
        public string? FaxNumber { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
    }

    public class EmergencyContactData
    {
        public string? RelationshipWithPatient { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }

    public class InsuranceData
    {
        public bool PaymentMethod { get; set; }
        public string? InsuranceType { get; set; }
        public string? InsuranceName { get; set; }
        public string? MemberId { get; set; }
        public string? PlanName { get; set; }
        public string? PlanType { get; set; }
        public string? GroupId { get; set; }
        public string? GroupName { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public string? PatientRelationshipWithInsured { get; set; }
        public string? InsuranceCard { get; internal set; }
        public string? InsuranceCard1 { get; internal set; }
    }

    public class OtherInfoData
    {
        public bool ConsentToEmail { get; set; }
        public bool ConsentToMessage { get; set; }
        public string? PracticeLocation { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public string? Source { get; set; }
        public string? SoberLivingHome { get; set; }
    }
}
