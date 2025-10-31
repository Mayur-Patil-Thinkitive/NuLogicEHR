using System.ComponentModel.DataAnnotations;
using NuLogicEHR.Enums;

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
        public Source? Source { get; set; }
        public SoberLivingHomeType? SoberLivingHome { get; set; }
        public int? SoberLivingHomeId { get; set; }
        public string? SoberLivingHomeName { get; set; }
        public SoberLivingHome? SoberLivingHomeNavigation { get; set; }
        public int PatientId { get; set; }
        public bool ?IsUsingNuLeaseTransportationService { get; set; }
        public PatientDemographic PatientDemographic { get; set; }//add forien key
        public DateTime? CreatedBy { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedBy { get; set; } = DateTime.UtcNow;
    }
}