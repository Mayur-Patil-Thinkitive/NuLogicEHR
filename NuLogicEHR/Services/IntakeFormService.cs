using NuLogicEHR.Configurations;
using NuLogicEHR.Models;
using NuLogicEHR.ViewModels;
using NuLogicEHR.Repository;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace NuLogicEHR.Services
{
    public class IntakeFormService : BaseService
    {
        private readonly ILogger<IntakeFormService> _logger;

        public IntakeFormService(TenantService tenantService, DbContextFactory dbContextFactory, ILogger<IntakeFormService> logger)
            : base(tenantService, dbContextFactory)
        {
            _logger = logger;
        }
        public async Task<int> CreateHistoryAsync(int tenantId, PatientIntakeHistoryViewModel dto)
        {
            _logger.LogInformation("CreateHistoryAsync called for TenantId: {TenantId}, PatientId: {PatientId}", tenantId, dto.PatientId);

            try
            {
                using var context = await GetContextAsync(tenantId);
                _logger.LogInformation("Database context successfully retrieved for TenantId: {TenantId}", tenantId);

                var repository = new IntakeFormRepository(context);
                _logger.LogInformation("IntakeFormRepository initialized for TenantId: {TenantId}", tenantId);

                var history = new PatientIntakeHistory
                {
                    PatientId = dto.PatientId,
                    MedicalHistoryJson = dto.MedicalHistory != null ? JsonSerializer.Serialize(dto.MedicalHistory) : null,
                    VaccineHistoryJson = dto.VaccineHistory != null ? JsonSerializer.Serialize(dto.VaccineHistory) : null,
                    SurgicalHistoryJson = dto.SurgicalHistory != null ? JsonSerializer.Serialize(dto.SurgicalHistory) : null,
                    FamilyHistoryJson = dto.FamilyHistory != null ? JsonSerializer.Serialize(dto.FamilyHistory) : null,
                    TobaccoUseJson = dto.TobaccoUse != null ? JsonSerializer.Serialize(dto.TobaccoUse) : null,
                    AlcoholUseJson = dto.AlcoholUse != null ? JsonSerializer.Serialize(dto.AlcoholUse) : null,
                    SubstanceUseJson = dto.SubstanceUse != null ? JsonSerializer.Serialize(dto.SubstanceUse) : null,
                    CurrentlyOnInjections = dto.CurrentlyOnInjections,
                    InjectionHistoryJson = dto.InjectionHistory != null ? JsonSerializer.Serialize(dto.InjectionHistory) : null
                };

                _logger.LogInformation("Prepared PatientIntakeHistory object for PatientId: {PatientId}", dto.PatientId);

                var result = await repository.CreateHistoryAsync(history);
                _logger.LogInformation("Patient intake history created successfully with Id: {HistoryId} for PatientId: {PatientId}", result, dto.PatientId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating patient intake history for TenantId: {TenantId}, PatientId: {PatientId}", tenantId, dto.PatientId);
                throw; // Preserve original exception behavior
            }
        }
        public async Task<PatientIntakeHistoryViewModel?> GetHistoryByIdAsync(int tenantId, int id)
        {
            _logger.LogInformation("GetHistoryByIdAsync called for TenantId: {TenantId}, HistoryId: {HistoryId}", tenantId, id);

            try
            {
                using var context = await GetContextAsync(tenantId);
                _logger.LogInformation("Database context retrieved successfully for TenantId: {TenantId}", tenantId);

                var repository = new IntakeFormRepository(context);
                _logger.LogInformation("IntakeFormRepository initialized for TenantId: {TenantId}", tenantId);

                var history = await repository.GetHistoryByIdAsync(id);

                if (history == null)
                {
                    _logger.LogWarning("No patient intake history found for HistoryId: {HistoryId} in TenantId: {TenantId}", id, tenantId);
                    return null;
                }

                _logger.LogInformation("Patient intake history found for PatientId: {PatientId}, HistoryId: {HistoryId}", history.PatientId, id);

                var result = new PatientIntakeHistoryViewModel
                {
                    PatientId = history.PatientId,
                    MedicalHistory = !string.IsNullOrEmpty(history.MedicalHistoryJson)
                        ? JsonSerializer.Deserialize<List<MedicalHistoryEntry>>(history.MedicalHistoryJson) : null,
                    VaccineHistory = !string.IsNullOrEmpty(history.VaccineHistoryJson)
                        ? JsonSerializer.Deserialize<List<VaccineEntry>>(history.VaccineHistoryJson) : null,
                    SurgicalHistory = !string.IsNullOrEmpty(history.SurgicalHistoryJson)
                        ? JsonSerializer.Deserialize<List<SurgicalHistoryEntry>>(history.SurgicalHistoryJson) : null,
                    FamilyHistory = !string.IsNullOrEmpty(history.FamilyHistoryJson)
                        ? JsonSerializer.Deserialize<List<FamilyHistoryEntry>>(history.FamilyHistoryJson) : null,
                    TobaccoUse = !string.IsNullOrEmpty(history.TobaccoUseJson)
                        ? JsonSerializer.Deserialize<List<TobaccoUseEntry>>(history.TobaccoUseJson) : null,
                    AlcoholUse = !string.IsNullOrEmpty(history.AlcoholUseJson)
                        ? JsonSerializer.Deserialize<List<AlcoholUseEntry>>(history.AlcoholUseJson) : null,
                    SubstanceUse = !string.IsNullOrEmpty(history.SubstanceUseJson)
                        ? JsonSerializer.Deserialize<List<SubstanceUseEntry>>(history.SubstanceUseJson) : null,
                    CurrentlyOnInjections = history.CurrentlyOnInjections,
                    InjectionHistory = !string.IsNullOrEmpty(history.InjectionHistoryJson)
                        ? JsonSerializer.Deserialize<List<InjectionEntry>>(history.InjectionHistoryJson) : null
                };

                _logger.LogInformation("Patient intake history data successfully deserialized for PatientId: {PatientId}, HistoryId: {HistoryId}", history.PatientId, id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving patient intake history for TenantId: {TenantId}, HistoryId: {HistoryId}", tenantId, id);
                throw;
            }
        }
        public async Task UpdateHistoryAsync(int tenantId, int id, PatientIntakeHistoryViewModel dto)
        {
            _logger.LogInformation("UpdateHistoryAsync called for TenantId: {TenantId}, HistoryId: {HistoryId}, PatientId: {PatientId}", tenantId, id, dto.PatientId);

            try
            {
                using var context = await GetContextAsync(tenantId);
                _logger.LogInformation("Database context retrieved successfully for TenantId: {TenantId}", tenantId);

                var repository = new IntakeFormRepository(context);
                _logger.LogInformation("IntakeFormRepository initialized for TenantId: {TenantId}", tenantId);

                var history = await repository.GetHistoryByIdAsync(id);

                if (history == null)
                {
                    _logger.LogWarning("Patient intake history not found for HistoryId: {HistoryId}, TenantId: {TenantId}", id, tenantId);
                    throw new InvalidOperationException("History not found");
                }

                _logger.LogInformation("Updating patient intake history for PatientId: {PatientId}, HistoryId: {HistoryId}", history.PatientId, id);

                history.MedicalHistoryJson = dto.MedicalHistory != null ? JsonSerializer.Serialize(dto.MedicalHistory) : null;
                history.VaccineHistoryJson = dto.VaccineHistory != null ? JsonSerializer.Serialize(dto.VaccineHistory) : null;
                history.SurgicalHistoryJson = dto.SurgicalHistory != null ? JsonSerializer.Serialize(dto.SurgicalHistory) : null;
                history.FamilyHistoryJson = dto.FamilyHistory != null ? JsonSerializer.Serialize(dto.FamilyHistory) : null;
                history.TobaccoUseJson = dto.TobaccoUse != null ? JsonSerializer.Serialize(dto.TobaccoUse) : null;
                history.AlcoholUseJson = dto.AlcoholUse != null ? JsonSerializer.Serialize(dto.AlcoholUse) : null;
                history.SubstanceUseJson = dto.SubstanceUse != null ? JsonSerializer.Serialize(dto.SubstanceUse) : null;
                history.CurrentlyOnInjections = dto.CurrentlyOnInjections;
                history.InjectionHistoryJson = dto.InjectionHistory != null ? JsonSerializer.Serialize(dto.InjectionHistory) : null;
                history.UpdatedAt = DateTime.UtcNow;

                await repository.UpdateHistoryAsync(history);
                _logger.LogInformation("Patient intake history updated successfully for HistoryId: {HistoryId}, PatientId: {PatientId}", id, history.PatientId);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Attempted to update non-existing history record for HistoryId: {HistoryId}, TenantId: {TenantId}", id, tenantId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating patient intake history for TenantId: {TenantId}, HistoryId: {HistoryId}", tenantId, id);
                throw;
            }
        }
        public async Task DeleteHistoryAsync(int tenantId, int id)
        {
            _logger.LogInformation("DeleteHistoryAsync called for TenantId: {TenantId}, HistoryId: {HistoryId}", tenantId, id);

            try
            {
                using var context = await GetContextAsync(tenantId);
                _logger.LogInformation("Database context retrieved successfully for TenantId: {TenantId}", tenantId);

                var repository = new IntakeFormRepository(context);
                _logger.LogInformation("IntakeFormRepository initialized for TenantId: {TenantId}", tenantId);

                await repository.DeleteHistoryAsync(id);
                _logger.LogInformation("Patient intake history deleted successfully for HistoryId: {HistoryId}, TenantId: {TenantId}", id, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting patient intake history for TenantId: {TenantId}, HistoryId: {HistoryId}", tenantId, id);
                throw;
            }
        }

    }
}
