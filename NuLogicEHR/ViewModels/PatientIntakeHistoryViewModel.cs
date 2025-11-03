using NuLogicEHR.Models;

namespace NuLogicEHR.ViewModels
{
    public class PatientIntakeHistoryViewModel
    {
        public int PatientId { get; set; }
        
        // Medical History
        public List<MedicalHistoryEntry>? MedicalHistory { get; set; }
        
        // Vaccine History
        public List<VaccineEntry>? VaccineHistory { get; set; }
        
        // Surgical History
        public List<SurgicalHistoryEntry>? SurgicalHistory { get; set; }
        
        // Family History
        public List<FamilyHistoryEntry>? FamilyHistory { get; set; }
        
        // Social History - Tobacco Use
        public List<TobaccoUseEntry>? TobaccoUse { get; set; }
        
        // Social History - Alcohol Use
        public List<AlcoholUseEntry>? AlcoholUse { get; set; }
        
        // Social History - Substance Use
        public List<SubstanceUseEntry>? SubstanceUse { get; set; }
        
        // Injection History
        public bool? CurrentlyOnInjections { get; set; }
        public List<InjectionEntry>? InjectionHistory { get; set; }
    }
}
