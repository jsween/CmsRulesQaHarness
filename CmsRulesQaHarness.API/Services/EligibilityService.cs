using CmsRulesQaHarness.API.Models;
using CmsRulesQaHarness.API.Models.Enums;

namespace CmsRulesQaHarness.API.Services
{
    /// <summary>
    /// Implements CMS program eligibility determination logic based on applicant information.
    /// </summary>
    public class EligibilityService : IEligibilityService
    {
        public EligibilityResult DetermineEligibility(EligibilityRequest request)
        {
            var result = new EligibilityResult();

            if (request.Age < 0)
            {
                result.IsEligible = false;
                result.ProgramCategory = ProgramCategory.Invalid;
                result.Reasons = [ValidationReasons.AgeNegative];
                return result;
            }

            if (request.HouseholdSize <= 0)
            {
                result.IsEligible = false;
                result.ProgramCategory = ProgramCategory.Invalid;
                result.Reasons.Add(ValidationReasons.HouseholdSizeInvalid);
                return result;
            }

            if (request.MonthlyIncome < 0)
            {
                result.IsEligible = false;
                result.ProgramCategory = ProgramCategory.Invalid;
                result.Reasons.Add(ValidationReasons.MonthlyIncomeNegative);
                return result;
            }

            if (request.Age >= 65 && request.MonthlyIncome <= 1600)
            {
                result.IsEligible = true;
                result.ProgramCategory = ProgramCategory.Medicare;
                result.Reasons.Add(ValidationReasons.Age65OrOlderIncomeThreshold);
                return result;
            }

            if (request.IsDisabled && request.MonthlyIncome <= 1800)
            {
                result.IsEligible = true;
                result.ProgramCategory = ProgramCategory.Medicaid;
                result.Reasons.Add(ValidationReasons.DisabledIncomeThreshold);
                return result;
            }

            if (request.IsPregnant && request.MonthlyIncome <= 2000)
            {
                result.IsEligible = true;
                result.ProgramCategory = ProgramCategory.CHIP;
                result.Reasons.Add(ValidationReasons.PregnantIncomeThreshold);
                return result;
            }

            if (request.IsBlind && request.MonthlyIncome <= 2200)
            {
                result.IsEligible = true;
                result.ProgramCategory = ProgramCategory.Medicaid;
                result.Reasons.Add(ValidationReasons.BlindIncomeThreshold);
                return result;
            }

            result.IsEligible = false;
            result.ProgramCategory = ProgramCategory.None;
            result.Reasons.Add(ValidationReasons.NotEligible);

            return result;
        }
    }
}
