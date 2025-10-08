using System.ComponentModel.DataAnnotations;

namespace NuLogicEHR.ViewModels
{
    public class SoberLivingHomeCreateViewModel
    {
        [Required]
        public string SoberLivingHomeName { get; set; }
        [Required]
        public string ContactPersonName { get; set; }
        [Required]
        public string EmailId { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? RegistrationNumber { get; set; }
        public bool Transportation { get; set; }
        [Required]
        public bool Status { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        [Required]
        public string City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
    }

    public class SoberLivingHomeResponseViewModel
    {
        public int Id { get; set; }
        public string SoberLivingHomeName { get; set; }
        public string ContactPersonName { get; set; }
        public string EmailId { get; set; }
        public string ContactNumber { get; set; }
        public string? FaxNumber { get; set; }
        public string? RegistrationNumber { get; set; }
        public bool Transportation { get; set; }
        public bool Status { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }
        public DateTime CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; }
    }
}
