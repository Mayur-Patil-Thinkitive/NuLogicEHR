using System.ComponentModel.DataAnnotations;

namespace NuLogicEHR.ViewModels
{
    public class PatientContactViewModel
    {
        [Required]
        public int PatientId { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        public string? HomeNumber { get; set; }
        public string? Email { get; set; }
        public string? EmailNote { get; set; }

        public string? FaxNumber { get; set; }
        [Required]
        public string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string State { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string ZipCode { get; set; }
    }
}
