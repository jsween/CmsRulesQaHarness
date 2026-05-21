using CmsRulesQaHarness.API.Models;
using CmsRulesQaHarness.API.Models.Enums;
using CmsRulesQaHarness.API.Services;

namespace CmsRulesQaHarness.Tests
{
    public class EligibilityRuleTests
    {
        private readonly EligibilityService _service = new();

        #region Positive Test Cases

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
        public void DetermineEligibility_DisabledApplicantWithinIncomeLimit_ReturnsEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 40,
                MonthlyIncome = 1500,
                IsDisabled = true,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicaid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.DisabledIncomeThreshold, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_PregnantApplicantWithinIncomeLimit_ReturnsEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 28,
                MonthlyIncome = 1800,
                IsDisabled = false,
                IsPregnant = true,
                IsBlind = false,
                HouseholdSize = 2
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.CHIP, result.ProgramCategory);
            Assert.Contains(ValidationReasons.PregnantIncomeThreshold, result.Reasons);
        }

        #endregion

        #region Negative Test Cases

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
        public void DetermineEligibility_MultipleConditionsIncomeTooHigh_ReturnsNotEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 50,
                MonthlyIncome = 3000,
                IsDisabled = true,
                IsPregnant = true,
                IsBlind = true,
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
        public void DetermineEligibility_NegativeHouseholdSize_ReturnsInvalid()
        {
            var request = new EligibilityRequest
            {
                Age = 40,
                MonthlyIncome = 1000,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = -2
            };

            var result = _service.DetermineEligibility(request);

            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.Invalid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.HouseholdSizeInvalid, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_NegativeMonthlyIncome_ReturnsInvalid()
        {
            var request = new EligibilityRequest
            {
                Age = 40,
                MonthlyIncome = -500,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.Invalid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.MonthlyIncomeNegative, result.Reasons);
        }

        #endregion

        #region Edge Case Tests

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

        [Fact]
        public void DetermineEligibility_ApplicantAtAge65_ReturnsEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 65,
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
        public void DetermineEligibility_ApplicantAtAge64_ReturnsNotEligibleForMedicare()
        {
            var request = new EligibilityRequest
            {
                Age = 64,
                MonthlyIncome = 1200,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
        }

        [Fact]
        public void DetermineEligibility_DisabledAtExactIncomeLimit_ReturnsEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 45,
                MonthlyIncome = 1800,
                IsDisabled = true,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicaid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.DisabledIncomeThreshold, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_DisabledOneDollarOverIncomeLimit_ReturnsNotEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 45,
                MonthlyIncome = 1801,
                IsDisabled = true,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
        }

        [Fact]
        public void DetermineEligibility_PregnantAtExactIncomeLimit_ReturnsEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 30,
                MonthlyIncome = 2000,
                IsDisabled = false,
                IsPregnant = true,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.CHIP, result.ProgramCategory);
            Assert.Contains(ValidationReasons.PregnantIncomeThreshold, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_PregnantOneDollarOverIncomeLimit_ReturnsNotEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 30,
                MonthlyIncome = 2001,
                IsDisabled = false,
                IsPregnant = true,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
        }

        [Fact]
        public void DetermineEligibility_BlindAtExactIncomeLimit_ReturnsEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 40,
                MonthlyIncome = 2200,
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
        public void DetermineEligibility_BlindOneDollarOverIncomeLimit_ReturnsNotEligible()
        {
            var request = new EligibilityRequest
            {
                Age = 40,
                MonthlyIncome = 2201,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = true,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
        }

        [Fact]
        public void DetermineEligibility_PregnantAndDisabled_ReturnsDisabledDueToRulePriority()
        {
            var request = new EligibilityRequest
            {
                Age = 32,
                MonthlyIncome = 1700,
                IsDisabled = true,
                IsPregnant = true,
                IsBlind = false,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicaid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.DisabledIncomeThreshold, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_BlindAndDisabled_ReturnsDisabledDueToRulePriority()
        {
            var request = new EligibilityRequest
            {
                Age = 38,
                MonthlyIncome = 1700,
                IsDisabled = true,
                IsPregnant = false,
                IsBlind = true,
                HouseholdSize = 1
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicaid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.DisabledIncomeThreshold, result.Reasons);
        }

        [Fact]
        public void DetermineEligibility_LargeHouseholdSize_ProcessesCorrectly()
        {
            var request = new EligibilityRequest
            {
                Age = 70,
                MonthlyIncome = 1500,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false,
                HouseholdSize = 8
            };

            var result = _service.DetermineEligibility(request);

            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicare, result.ProgramCategory);
        }

        #endregion
    }
}
