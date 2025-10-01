namespace NuLogicEHR.ViewModels
{
    public class ImportPatientsResultViewModel
    {
        public int ImportedCount { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
