using System.ComponentModel.DataAnnotations;

namespace NuLogicEHR.ViewModels
{
    public class InsuranceInformationViewModel
    {
        [Required]
        public int PatientId { get; set; }
        public string? InsuranceType { get; set; }
        public string? InsuranceName { get; set; }
        public string? MemberId { get; set; }
        public string? PlanName { get; set; }
        public string? PlanType { get; set; }
        public string? GroupId { get; set; }
        public string? GroupName { get; set; }
        public DateTime? EffectiveStartDate { get; set; }
        public DateTime? EffectiveEndDate { get; set; }
        public string? PatientRelationshipWithInsured { get; set; }
        public string? InsuranceCardFilePath { get; set; }
    }
}
