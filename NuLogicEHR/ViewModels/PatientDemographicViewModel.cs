using System.ComponentModel.DataAnnotations;

namespace NuLogicEHR.ViewModels
{
    public class PatientDemographicViewModel
    {
        public string? ProfileImagePath { get; set; }
        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string? Suffix { get; set; }
        public string? Nickname { get; set; }
        [Required]
        public string GenderAtBirth { get; set; }
        [Required]
        public string CurrentGender { get; set; }
        public string? Pronouns { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        public string? MaritalStatus { get; set; }
        public string? TimeZone { get; set; }
        [Required]
        public string PreferredLanguage { get; set; }
        public string? Occupation { get; set; }
        [Required]
        public int? SSN { get; set; }

        public string? SSNNote { get; set; }
        [Required]
        public string Race { get; set; }
        public string? Ethnicity { get; set; }
        public string? TreatmentType { get; set; }
        public DateTime? CreatedBy { get; set; } = DateTime.UtcNow;

    }
}
