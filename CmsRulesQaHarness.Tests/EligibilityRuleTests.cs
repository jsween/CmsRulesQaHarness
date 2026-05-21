using CmsRulesQaHarness.API.Models;
using CmsRulesQaHarness.API.Models.Enums;
using CmsRulesQaHarness.API.Services;

namespace CmsRulesQaHarness.Tests
{
    public class EligibilityRuleTests
    {
        private readonly EligibilityService _service = new();
        
        [Fact]
        public void DetermineEligibility_AgedApplicantWithinIncomeLimit_ReturnsEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 67,
                MonthlyIncome = 1200,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicare, result.ProgramCategory);
            Assert.Contains(ValidationReasons.Age65OrOlderIncomeThreshold, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_BlindApplicantWithinIncomeLimit_ReturnsEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 35,
                MonthlyIncome = 1500,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = true,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicaid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.BlindIncomeThreshold, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_AgedApplicantIncomeTooHigh_ReturnsNotEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 67,
                MonthlyIncome = 2500,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
            Assert.Contains(ValidationReasons.NotEligible, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_NegativeAge_ReturnsInvalid()
        {
            var request = new EligibilityRequest
            {
                Age = -1,
                MonthlyIncome = 1000,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.Invalid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.AgeNegative, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_HouseholdSizeZero_ReturnsInvalid()
        {
            var request = new EligibilityRequest
            {
                Age = 40,
                MonthlyIncome = 1000,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 0
            };

            var result = _service.DetermineEligibility(request);

            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.Invalid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.HouseholdSizeInvalid, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_AgedApplicantAtExactIncomeLimit_ReturnsEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 70,
                MonthlyIncome = 1600,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicare, result.ProgramCategory);
            Assert.Contains(ValidationReasons.Age65OrOlderIncomeThreshold, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_AgedApplicantOneDollarOverIncomeLimit_ReturnsNotEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 70,
                MonthlyIncome = 1601,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
            Assert.Contains(ValidationReasons.NotEligible, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_AgedAndDisabledApplicant_ReturnsAgedDueToRulePriority()
        {
            var request = new EligibilityRequest
            {
                Age = 68,
                MonthlyIncome = 1500,
                IsDisabled = true,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicare, result.ProgramCategory);
            Assert.Contains(ValidationReasons.Age65OrOlderIncomeThreshold, result.Reasons);
        }
    }
}
