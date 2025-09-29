using NuLogicEHR.Enums;

namespace NuLogicEHR.ViewModels
{
    public class AppointmentResponseViewModel
    {
        public int Id { get; set; }
        public string PatientName { get; set; }
        public bool AppointmentMode { get; set; }
        public string TreatmentType { get; set; }
        public string AppointmentType { get; set; }
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; }
        public List<string> SelectedForms { get; set; }
        public bool TransportationService { get; set; }
    }
}
