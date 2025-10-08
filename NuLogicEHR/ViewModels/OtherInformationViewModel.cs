using System.ComponentModel.DataAnnotations;
using NuLogicEHR.Enums;
using NuLogicEHR.Models;

namespace NuLogicEHR.ViewModels
{
    public class OtherInformationViewModel
    {
        [Required]
        public int PatientId { get; set; }
        public bool ConsentToEmail { get; set; }
        public bool ConsentToMessage { get; set; }
        public string? PracticeLocation { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public Source? Source { get; set; }
        public SoberLivingHomeType? SoberLivingHome { get; set; }
    }
}