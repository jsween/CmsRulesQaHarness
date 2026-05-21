namespace CmsRulesQaHarness.API.Models
{
    /// <summary>
    /// Contains predefined validation reason messages for eligibility determination.
    /// </summary>
    public class ValidationReasons
    {
        public const string NotEligible = "Applicant does not meet eligibility criteria for any CMS program.";
        public const string AgeNegative = "Age cannot be negative.";
        public const string HouseholdSizeInvalid = "Household size must be greater than zero.";
        public const string MonthlyIncomeNegative = "Monthly income cannot be negative.";
        public const string Age65OrOlderIncomeThreshold = "Applicant is age 65 or older and within income threshold.";
        public const string DisabledIncomeThreshold = "Applicant is disabled and within income threshold.";
        public const string PregnantIncomeThreshold = "Applicant is pregnant and within income threshold.";
        public const string BlindIncomeThreshold = "Applicant is blind and within income threshold.";
    }
}
