using System.ComponentModel.DataAnnotations;
using NuLogicEHR.Enums;


namespace NuLogicEHR.Models
{
    public class Staff
    {
        [Key]
        public int Id { get; set; }
        public string? ProfileImage { get; set; }

        [Required]
        public StaffRoleType Role { get; set; } // Only FrontDesk or Biller

        [Required]
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public bool KioskAccess { get; set; }

        [Range(1000, 9999, ErrorMessage = "PIN must be a 4-digit number")]
        public int? KioskPin { get; set; }

        public string? MailingAddressLine1 { get; set; }
        public string? MailingAddressLine2 { get; set; }
        public string? BillerId { get; set; }

        public int? TaxIdNumber { get; set; }
        public string? NationalProviderId { get; set; }
        public string? BillingLicenseNumber { get; set; }
        public string? IssuingState { get; set; }

        public int? MedicareProviderNumber { get; set; }
        public string? MedicareProviderDoc { get; set; }
        public string? ClearinghouseId { get; set; }

        public string? ERAEFTStatus { get; set; }
        public string? PayerAssignedId { get; set; }
        public string? BankRoutingNumber { get; set; }
        public string? AccountNumber { get; set; }
        public string? CAQHProviderId { get; set; }

        public DateTime? LastLogin { get; set; }

        [Required]
        public string Status { get; set; }

        public string? Signature { get; set; }

        // Navigation property for credentials
        public ICollection<StaffCredential> Credentials { get; set; } = new List<StaffCredential>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedAt { get; set; } = DateTime.UtcNow;
    }
}
