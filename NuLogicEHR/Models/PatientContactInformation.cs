using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace NuLogicEHR.Models
{
    public class PatientContactInformation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string MobileNumber { get; set; }
        public string? HomeNumber { get; set; }

        [Required]
        public string Email { get; set; }

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

        [DataMember]
        public int PatientId { get; set; }

        [DataMember]
        public PatientDemographic PatientDemographic { get; set; }
        public DateTime? CreatedBy { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedBy { get; set; } = DateTime.UtcNow;
    }
}