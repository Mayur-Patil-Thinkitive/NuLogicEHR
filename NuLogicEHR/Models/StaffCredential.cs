using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

using System.Text.Json.Serialization;
using NuLogicEHR.Enums;

namespace NuLogicEHR.Models
{
    public class StaffCredential
    {
        [Key]
        public int Id { get; set; }
        public CredentialType CredentialType { get; set; }
        public string CredentialNumber { get; set; }
        public int StaffId { get; set; }
        [JsonIgnore] // Ignore this navigation property to prevent circular reference
        public Staff Staff { get; set; }
        public DateTime? CreatedBy { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedBy { get; set; } = DateTime.UtcNow;
    }
}
