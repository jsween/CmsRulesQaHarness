using CmsRulesQaHarness.API.Models;
using CmsRulesQaHarness.API.Models.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace CmsRulesQaHarness.Tests
{
    public class EligibilityApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        #region Positive Cases

        [Fact]
        public async Task DetermineEligibility_WithValidMedicareApplicant_ReturnsEligibleResult()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 67,
                MonthlyIncome = 1200,
                HouseholdSize = 1,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicare, result.ProgramCategory);
            Assert.Contains(ValidationReasons.Age65OrOlderIncomeThreshold, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_WithDisabledApplicant_ReturnsEligibleMedicaidResult()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 40,
                MonthlyIncome = 1500,
                HouseholdSize = 1,
                IsDisabled = true,
                IsPregnant = false,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicaid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.DisabledIncomeThreshold, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_WithPregnantApplicant_ReturnsEligibleCHIPResult()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 28,
                MonthlyIncome = 1800,
                HouseholdSize = 2,
                IsDisabled = false,
                IsPregnant = true,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.CHIP, result.ProgramCategory);
            Assert.Contains(ValidationReasons.PregnantIncomeThreshold, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_WithBlindApplicant_ReturnsEligibleMedicaidResult()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 35,
                MonthlyIncome = 2000,
                HouseholdSize = 1,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = true
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicaid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.BlindIncomeThreshold, result.Reasons);
        }

        #endregion

        #region Negative Cases

        [Fact]
        public async Task DetermineEligibility_WithOverIncomeApplicant_ReturnsNotEligibleResult()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 67,
                MonthlyIncome = 5000,
                HouseholdSize = 1,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
            Assert.Contains(ValidationReasons.NotEligible, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_WithInvalidAge_ReturnsInvalidResult()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = -1,
                MonthlyIncome = 1000,
                HouseholdSize = 1,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.Invalid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.AgeNegative, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_WithInvalidHouseholdSize_ReturnsInvalidResult()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 40,
                MonthlyIncome = 1000,
                HouseholdSize = 0,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.Invalid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.HouseholdSizeInvalid, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_WithNegativeIncome_ReturnsInvalidResult()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 40,
                MonthlyIncome = -500,
                HouseholdSize = 1,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.Invalid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.MonthlyIncomeNegative, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_WithMalformedJson_ReturnsBadRequest()
        {
            // Arrange
            var malformedJson = new StringContent(
                "{ \"age\": 67, \"monthlyIncome\": ",
                System.Text.Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", malformedJson);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task DetermineEligibility_WithMedicareAtExactIncomeLimit_ReturnsEligibleResult()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 70,
                MonthlyIncome = 1600,
                HouseholdSize = 1,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicare, result.ProgramCategory);
        }

        [Fact]
        public async Task DetermineEligibility_WithMedicareOneDollarOverLimit_ReturnsNotEligible()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 70,
                MonthlyIncome = 1601,
                HouseholdSize = 1,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
        }

        [Fact]
        public async Task DetermineEligibility_WithAge65Boundary_ReturnsEligibleMedicare()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 65,
                MonthlyIncome = 1200,
                HouseholdSize = 1,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicare, result.ProgramCategory);
        }

        [Fact]
        public async Task DetermineEligibility_WithAge64_ReturnsNotEligibleForMedicare()
        {
            // Arrange
            var request = new EligibilityRequest
            {
                Age = 64,
                MonthlyIncome = 1200,
                HouseholdSize = 1,
                IsDisabled = false,
                IsPregnant = false,
                IsBlind = false
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
        }

        #endregion
    }
}
