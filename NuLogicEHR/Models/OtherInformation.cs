using System.ComponentModel.DataAnnotations;

namespace NuLogicEHR.Models
{
    public class OtherInformation
    {
        [Key]
        public int Id { get; set; }

        public bool? ConsentToEmail { get; set; }

        public bool? ConsentToMessage { get; set; }

        public string? PracticeLocation { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public string? Source { get; set; }
        public int PatientId { get; set; }
        public PatientDemographic PatientDemographic { get; set; }//add forien key
        public DateTime? CreatedBy { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedBy { get; set; } = DateTime.UtcNow;
    }
}