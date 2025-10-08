using System.ComponentModel.DataAnnotations;
using NuLogicEHR.Enums;
using NuLogicEHR.Configurations;

namespace NuLogicEHR.Models
{
    public class Provider
    {
        [Key]
        public int Id { get; set; }

        public string? ProfileImage { get; set; }

        [Required]
        public string FirstName { get; set; }

        public string? MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string EmailId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        public string? OfficeFaxNumber { get; set; }
        
        [Required]
        public string Gender { get; set; }
        [Required]
        public ProviderType ProviderType { get; set; }
        [Required]
        public ProviderRoleType Role { get; set; }

        [Required]
        public string NPINumber { get; set; }
        [Required]
        public string GroupNPINumber { get; set; }
        [Required]
        public LicensedState LicensedState { get; set; }

        [Required]
        public string LicenseNumber { get; set; }
        public string? TaxonomyNumber { get; set; }
        [Required]
        public LocationType WorkLocation { get; set; }
        public string? InsuranceAccepted { get; set; }
        [Required]
        public int YearsOfExperience { get; set; }
        public string? DEANumber { get; set; }
        [Required]
        public string Status { get; set; }
        public bool? MapRenderingProvider { get; set; }
        public bool? KioskAccess { get; set; }

        [Range(1000, 9999, ErrorMessage = "PIN must be a 4-digit number")]
        public int? KioskPin { get; set; }
        public string? Bio { get; set; }
        public string? Signature { get; set; }
        public DateTime? CreatedBy { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedBy { get; set; } = DateTime.UtcNow;
    }
}
