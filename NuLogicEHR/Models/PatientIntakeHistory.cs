using System.ComponentModel.DataAnnotations;

namespace NuLogicEHR.Models
{
    public class PatientIntakeHistory
    {
        [Key]
        public int Id { get; set; }
        
        public int PatientId { get; set; }
        public PatientDemographic PatientDemographic { get; set; }
        
        // Medical History (JSON stored as string for multiple entries)
        public string? MedicalHistoryJson { get; set; }
        
        // Vaccine History (JSON stored as string for multiple entries)
        public string? VaccineHistoryJson { get; set; }
        
        // Surgical History (JSON stored as string for multiple entries)
        public string? SurgicalHistoryJson { get; set; }
        
        // Family History (JSON stored as string for multiple entries)
        public string? FamilyHistoryJson { get; set; }
        
        // Social History - Tobacco Use (JSON for multiple entries)
        public string? TobaccoUseJson { get; set; }
        
        // Social History - Alcohol Use (JSON for multiple entries)
        public string? AlcoholUseJson { get; set; }
        
        // Social History - Substance Use (JSON for multiple entries)
        public string? SubstanceUseJson { get; set; }
        
        // Injection History
        public bool? CurrentlyOnInjections { get; set; }
        public string? InjectionHistoryJson { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
    
    // Supporting classes for structured data
    public class MedicalHistoryEntry
    {
        public string? ConditionName { get; set; }
        public DateTime? OnsetDate { get; set; }
        public string? Note { get; set; }
    }
    
    public class VaccineEntry
    {
        public string? VaccineName { get; set; }
        public int? YearAdministered { get; set; }
        public string? Note { get; set; }
    }
    
    public class SurgicalHistoryEntry
    {
        public string? SurgeryName { get; set; }
        public DateTime? OnsetDate { get; set; }
        public string? Note { get; set; }
    }
    
    public class FamilyHistoryEntry
    {
        public string? ConditionName { get; set; }
        public string? RelationWithPatient { get; set; }
        public DateTime? OnsetDate { get; set; }
        public string? Note { get; set; }
    }
    
    public class TobaccoUseEntry
    {
        public string? TobaccoType { get; set; }
        public string? DailyIntake { get; set; }
        public int? YearStarted { get; set; }
        public int? YearStopped { get; set; }
    }
    
    public class AlcoholUseEntry
    {
        public string? AlcoholType { get; set; }
        public string? DailyIntake { get; set; }
        public int? YearStarted { get; set; }
        public int? YearStopped { get; set; }
    }
    
    public class SubstanceUseEntry
    {
        public string? SubstanceType { get; set; }
        public string? RouteOfAdministration { get; set; }
        public string? DailyIntake { get; set; }
        public int? YearStarted { get; set; }
        public int? YearStopped { get; set; }
    }
    
    public class InjectionEntry
    {
        public string? Injection { get; set; }
        public string? FromHowLong { get; set; }
        public string? Note { get; set; }
    }
}
