using System.ComponentModel.DataAnnotations;

namespace NuLogicEHR.ViewModels
{
    public class EmergencyContactModelViewModel
    {
        [Required]
        public int PatientId { get; set; }
        public string? RelationshipWithPatient { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
    }
}
