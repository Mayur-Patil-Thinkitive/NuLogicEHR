namespace NuLogicEHR.ViewModels
{
    public class PatientIntakeProfileViewModel
    {
        public int PatientId { get; set; }

        // Demographics
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? Suffix { get; set; }
        public string? Nickname { get; set; }
        public string? GenderAtBirth { get; set; }
        public string? CurrentGender { get; set; }
        public string? Pronouns { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? MaritalStatus { get; set; }
        public string? TimeZone { get; set; }
        public string? PreferredLanguage { get; set; }
        public string? Occupation { get; set; }
        public string? SSN { get; set; }
        public string? Race { get; set; }
        public string? Ethnicity { get; set; }
        public string? TreatmentType { get; set; }

        // Contact
        public string? MobileNumber { get; set; }
        public string? HomeNumber { get; set; }
        public string? EmailId { get; set; }
        public string? FaxNumber { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }

        // Emergency Contact
        public string? EmergencyContactRelationship { get; set; }
        public string? EmergencyContactFirstName { get; set; }
        public string? EmergencyContactLastName { get; set; }
        public string? EmergencyContactPhoneNumber { get; set; }
        public string? EmergencyContactEmail { get; set; }

        // Insurance
        public string? PaymentMethod { get; set; }
        public string? InsuranceType { get; set; }
        public string? InsuranceName { get; set; }
        public string? MemberId { get; set; }
        public string? PlanName { get; set; }
        public string? PlanType { get; set; }
        public string? GroupId { get; set; }
        public string? GroupName { get; set; }
        public DateTime? EffectiveStartEndDate { get; set; }
        public string? PatientRelationshipWithInsured { get; set; }
        public string? InsuranceCardBack { get; set; }
        public string? InsuranceCardFront { get; set; }
        public bool HasSecondaryInsurance { get; set; }

        // Transportation
        public bool UsingNuLeaseTransportation { get; set; }
    }
}
