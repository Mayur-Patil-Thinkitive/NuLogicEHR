using System.ComponentModel.DataAnnotations;
using NuLogicEHR.Enums;

namespace NuLogicEHR.Models
{
    public class SchedulingAppointment
    {
        [Key]
        public int Id { get; set; }
        public bool AppointmentMode { get; set; } // false=InPerson, true=Virtual
        public TreatmentType TreatmentType { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public LocationType Location { get; set; }
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; } // "10:00 AM - 10:40 AM"
        public string SelectedForms { get; set; } // JSON array of FormType enums
        public bool TransportationService { get; set; }
        public int PatientId { get; set; }
        public PatientDemographic PatientDemographic { get; set; }
        public DateTime CreatedBy { get; set; } = DateTime.UtcNow;
        public DateTime ModifiedBy { get; set; } = DateTime.UtcNow;
    }
}
