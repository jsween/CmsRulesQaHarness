using System.Diagnostics;

namespace CmsRulesQaHarness.API.Models
{
    /// <summary>
    /// Represents an eligibility determination request containing demographic and financial
    /// information used to evaluate CMS program eligibility.
    /// </summary>
    public class EligibilityRequest
    {
        public int Age { get; set; }
        public int MonthlyIncome { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsPregnant { get; set; }
        public bool IsBlind { get; set; }
        public int HouseholdSize { get; set; }
    }
}
