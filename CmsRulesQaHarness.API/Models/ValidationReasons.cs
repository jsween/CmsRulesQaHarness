namespace CmsRulesQaHarness.API.Models
{
    /// <summary>
    /// Contains predefined validation reason messages for eligibility determination.
    /// </summary>
    public class ValidationReasons
    {
        public const string AgeNegative = "Age cannot be negative.";
        public const string HouseholdSizeInvalid = "Household size must be greater than zero.";
        public const string MonthlyIncomeNegative = "Monthly income cannot be negative.";
    }
}
