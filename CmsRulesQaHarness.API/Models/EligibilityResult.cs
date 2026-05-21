namespace CmsRulesQaHarness.API.Models
{
    /// <summary>
    /// Represents the result of a CMS program eligibility determination.
    /// </summary>
    public class EligibilityResult
    {
        public bool IsEligible { get; set; }
        public string ProgramCategory { get; set; } = string.Empty;
        public List<string> Reasons { get; set; } = [];
    }
}
