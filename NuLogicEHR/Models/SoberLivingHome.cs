using Microsoft.AspNetCore.Http.HttpResults;
using NuLogicEHR.Enums;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Metrics;

namespace NuLogicEHR.Models
{
    public class SoberLivingHome
    {
        [Key]
        public int Id { get; set; }
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
        public DateTime CreatedBy { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedBy { get; set; } = DateTime.UtcNow;
    }
}
