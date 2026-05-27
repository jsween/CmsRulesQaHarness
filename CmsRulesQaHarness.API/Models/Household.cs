namespace CmsRulesQaHarness.API.Models
{
    public class Household
    {
        public int HouseholdId { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public int HouseholdSize { get; set; }
        public string State { get; set; } = "WA";
        public string County { get; set; } = string.Empty;
         public List<Applicant> Applicants { get; set; } = [];
    }
}
