using CsvHelper.Configuration.Attributes;

namespace NuLogicEHR.ViewModels
{
    public class PatientCsvImportViewModel
    {
        // PatientDemographic fields from Excel
        [Name("FirstName")]
        public string FirstName { get; set; }
        
        [Name("LastName")]
        public string LastName { get; set; }
        
        [Name("DateOfBirth")]
        public DateTime DateOfBirth { get; set; }
        
        [Name("GenderAtBirth")]
        public string GenderAtBirth { get; set; }
        
        [Name("CurrentGender")]
        public string CurrentGender { get; set; }
        
        [Name("SSN")]
        public int? SSN { get; set; }
        
        [Name("TreatmentType")]
        public string? TreatmentType { get; set; }

        [Name("Email")]
        public string? Email { get; set; }
        // Insurance Information fields (only name and member id)
        [Name("InsuranceName")]
        public string? InsuranceName { get; set; }
        
        [Name("MemberId")]
        public string? MemberId { get; set; }
    }
}
