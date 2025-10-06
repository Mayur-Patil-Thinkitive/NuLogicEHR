using System.ComponentModel.DataAnnotations;
using NuLogicEHR.Enums;

namespace NuLogicEHR.DTOs
{
    public class StaffCredentialViewModel
    {
     
        public int CredentialType { get; set; } // 0 = ControlSubstanceLicense, 1 = Dummy
        public string CredentialNumber { get; set; }
    }

    public class CreateStaffViewModel
    {
        [Required]
        public int Role { get; set; } // 0 = FrontDesk, 1 = Biller

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string EmailId { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        public bool KioskAccess { get; set; }

        [Range(1000, 9999, ErrorMessage = "PIN must be a 4-digit number")]
        public int? KioskPin { get; set; }

        public string? MailingAddressLine1 { get; set; }
        public string? MailingAddressLine2 { get; set; }
        public string? BillerId { get; set; }

        // Credentials (only for Biller role)
        public List<StaffCredentialViewModel>? Credentials { get; set; }

        // Tax Information & Licensing (only for Biller role)
        public string? TaxIdNumber { get; set; }
        public string? NationalProviderId { get; set; }
        public string? BillingLicenseNumber { get; set; }
        public string? IssuingState { get; set; }

        public string? MedicareProviderNumber { get; set; }
        public IFormFile? MedicareProviderDoc { get; set; }
        public string? ClearinghouseId { get; set; }

        // Electronic Processing Setup (only for Biller role)
        public string? ERAEFTStatus { get; set; }
        public string? PayerAssignedId { get; set; }
        public string? BankRoutingNumber { get; set; }
        public string? AccountNumber { get; set; }
        public string? CAQHProviderId { get; set; }

        [Required]
        public string Status { get; set; }

    }

    public class StaffResponseDto
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}