using CmsRulesQaHarness.API.Models;

namespace CmsRulesQaHarness.Tests.TestData
{
    public static class EligibilityTestScenarios
    {
        // Positive Cases - Eligible Applicants
        public static EligibilityRequest MedicareEligibleApplicant => new()
        {
            Age = 67,
            MonthlyIncome = 1200,
            IsDisabled = false,
            IsPregnant = false,
            IsBlind = false,
            HouseholdSize = 1
        };

        public static EligibilityRequest DisabledEligibleApplicant => new()
        {
            Age = 40,
            MonthlyIncome = 1500,
            IsDisabled = true,
            IsPregnant = false,
            IsBlind = false,
            HouseholdSize = 1
        };

        public static EligibilityRequest PregnantEligibleApplicant => new()
        {
            Age = 28,
            MonthlyIncome = 1800,
            IsDisabled = false,
            IsPregnant = true,
            IsBlind = false,
            HouseholdSize = 2
        };

        public static EligibilityRequest BlindEligibleApplicant => new()
        {
            Age = 35,
            MonthlyIncome = 2000,
            IsDisabled = false,
            IsPregnant = false,
            IsBlind = true,
            HouseholdSize = 1
        };

        // Negative Cases - Not Eligible Applicants
        public static EligibilityRequest OverIncomeApplicant => new()
        {
            Age = 67,
            MonthlyIncome = 5000,
            IsDisabled = false,
            IsPregnant = false,
            IsBlind = false,
            HouseholdSize = 1
        };

        public static EligibilityRequest InvalidAgeApplicant => new()
        {
            Age = -1,
            MonthlyIncome = 1000,
            IsDisabled = false,
            IsPregnant = false,
            IsBlind = false,
            HouseholdSize = 1
        };

        public static EligibilityRequest InvalidHouseholdSizeApplicant => new()
        {
            Age = 40,
            MonthlyIncome = 1000,
            IsDisabled = false,
            IsPregnant = false,
            IsBlind = false,
            HouseholdSize = 0
        };

        public static EligibilityRequest NegativeIncomeApplicant => new()
        {
            Age = 40,
            MonthlyIncome = -500,
            IsDisabled = false,
            IsPregnant = false,
            IsBlind = false,
            HouseholdSize = 1
        };

        // Edge Cases - Boundary Conditions
        public static EligibilityRequest MedicareAtExactIncomeLimitApplicant => new()
        {
            Age = 70,
            MonthlyIncome = 1600,
            IsDisabled = false,
            IsPregnant = false,
            IsBlind = false,
            HouseholdSize = 1
        };

        public static EligibilityRequest MedicareOneDollarOverLimitApplicant => new()
        {
            Age = 70,
            MonthlyIncome = 1601,
            IsDisabled = false,
            IsPregnant = false,
            IsBlind = false,
            HouseholdSize = 1
        };

        public static EligibilityRequest Age65BoundaryApplicant => new()
        {
            Age = 65,
            MonthlyIncome = 1200,
            IsDisabled = false,
            IsPregnant = false,
            IsBlind = false,
            HouseholdSize = 1
        };

        public static EligibilityRequest Age64BoundaryApplicant => new()
        {
            Age = 64,
            MonthlyIncome = 1200,
            IsDisabled = false,
            IsPregnant = false,
            IsBlind = false,
            HouseholdSize = 1
        };
    }
}
