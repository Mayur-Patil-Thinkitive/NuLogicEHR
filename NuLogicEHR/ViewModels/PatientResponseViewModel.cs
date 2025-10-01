namespace NuLogicEHR.ViewModels
{
    public class PatientResponseViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string DateOfBirthFormatted => DateOfBirth.ToString("MM-dd-yyyy");
        public string Email { get; set; }
        public int TenantId { get; set; }
    }
}