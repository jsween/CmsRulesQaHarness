using CmsRulesQaHarness.API.Models.Enums;

namespace CmsRulesQaHarness.API.Models
{
    /// <summary>
    /// Represents the result of a CMS program eligibility determination.
    /// </summary>
    public class EligibilityResult
    {
        public bool IsEligible { get; set; }
        public ProgramCategory ProgramCategory { get; set; } = ProgramCategory.None;
        public List<string> Reasons { get; set; } = [];
    }
}
