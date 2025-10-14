using System.ComponentModel.DataAnnotations;

namespace NuLogicEHR.ViewModels
{
    public class BulkEmergencyContactViewModel
    {
        [Required]
        public int PatientId { get; set; }
        
        [Required]
        public List<EmergencyContactData> EmergencyContacts { get; set; } = new();
    }
}
