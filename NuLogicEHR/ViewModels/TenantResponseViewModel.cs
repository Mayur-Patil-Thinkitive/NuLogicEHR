namespace NuLogicEHR.ViewModels
{
    public class TenantResponseViewModel
    {
        public int Id { get; set; }
        public string HospitalName { get; set; }
        public DateTime CreatedBy { get; set; }
        public DateTime? ModifiedBy { get; set; } = DateTime.UtcNow;

    }
}