using Microsoft.EntityFrameworkCore;
using NuLogicEHR.Configurations;
using NuLogicEHR.Models;
using NuLogicEHR.ViewModels;
using NuLogicEHR.Enums;
using System.Text.Json;

namespace NuLogicEHR.Services
{
    public class AppointmentService : BaseService
    {
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(TenantService tenantService, DbContextFactory dbContextFactory, ILogger<AppointmentService> logger) 
            : base(tenantService, dbContextFactory)
        {
            _logger = logger;
        }

        public async Task<int> CreateAppointmentAsync(int tenantId, AppointmentCreateViewModel get)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);

                var appointment = new SchedulingAppointment
                {
                    AppointmentMode = get.AppointmentMode,
                    TreatmentType = get.TreatmentType,
                    AppointmentType = get.AppointmentType,
                    Location = get.Location,
                    Date = DateTime.SpecifyKind(get.Date, DateTimeKind.Utc),
                    TimeSlot = get.TimeSlot,
                    SelectedForms = JsonSerializer.Serialize(get.SelectedForms),
                    TransportationService = get.TransportationService,
                    PatientId = get.PatientId
                };

                context.SchedulingAppointments.Add(appointment);
                await context.SaveChangesAsync();

                return appointment.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAppointmentAsync for TenantId: {TenantId}", tenantId);
                throw new Exception("An error occurred while processing the appointment.", ex);
            }
        }

        public async Task<List<AppointmentResponseViewModel>> GetAppointmentsAsync(int tenantId)
        {
            try
            {
                using var context = await GetContextAsync(tenantId);

                var appointments = await context.SchedulingAppointments
                    .Include(a => a.PatientDemographic)
                    .ToListAsync();

                return appointments.Select(a => new AppointmentResponseViewModel
                {
                    Id = a.Id,
                    PatientName = a.PatientDemographic.FirstName + " " + a.PatientDemographic.LastName,
                    AppointmentMode = a.AppointmentMode,
                    TreatmentType = a.TreatmentType.ToString(),
                    AppointmentType = a.AppointmentType.ToString(),
                    Location = a.Location.ToString(),
                    Date = a.Date,
                    DateFormatted = a.Date.ToString("MM-dd-yyyy"),//Modify fix format mm/dd/yy format
                    TimeSlot = a.TimeSlot,
                    SelectedForms = string.IsNullOrEmpty(a.SelectedForms) ? new List<string>() :
                    JsonSerializer.Deserialize<List<FormType>>(a.SelectedForms).Select(f => f.ToString()).ToList(),
                    TransportationService = a.TransportationService
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAppointmentsAsync for TenantId: {TenantId}", tenantId);
                throw new Exception("An error occurred while retrieving appointments.", ex);
            }
        }
    }
}