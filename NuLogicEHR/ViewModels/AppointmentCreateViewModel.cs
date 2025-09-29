using System.ComponentModel.DataAnnotations;
using NuLogicEHR.Enums;

namespace NuLogicEHR.ViewModels
{
    public class AppointmentCreateViewModel
    {
        [Required]
        public bool AppointmentMode { get; set; }
        [Required]
        public TreatmentType TreatmentType { get; set; }
        [Required]
        public AppointmentType AppointmentType { get; set; }
        [Required]
        public LocationType Location { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string TimeSlot { get; set; }
        public List<FormType> SelectedForms { get; set; } = new();
        public bool TransportationService { get; set; }
        [Required]
        public int PatientId { get; set; }
    }
}
