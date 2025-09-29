namespace NuLogicEHR.Models
{
    public class Tenant
    {
        public int Id { get; set; }
        public string HospitalName { get; set; }
        public string SchemaName { get; set; }
        public DateTime CreatedBy { get; set; }
        // No Patients collection - schema isolation handles relationship
    }
}