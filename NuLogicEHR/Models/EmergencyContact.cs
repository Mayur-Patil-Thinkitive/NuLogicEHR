using System.ComponentModel.DataAnnotations;

namespace NuLogicEHR.Models
{
    public class EmergencyContact
    {
        [Key]
        public int Id { get; set; }

        public string? RelationshipWithPatient { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public int PatientId { get; set; }
        public PatientDemographic PatientDemographic { get; set; }
        public DateTime? CreatedBy { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedBy { get; set; } = DateTime.UtcNow;
    }
}