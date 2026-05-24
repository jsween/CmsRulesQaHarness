using CmsRulesQaHarness.API.Models;
using CmsRulesQaHarness.API.Models.Enums;
using CmsRulesQaHarness.Tests.TestData;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

namespace CmsRulesQaHarness.Tests.Api
{
    public class EligibilityApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        #region Positive Cases

        [Fact]
        public async Task DetermineEligibility_WithValidMedicareApplicant_ReturnsEligibleResult()
        {
            // Arrange
            var request = EligibilityTestScenarios.MedicareEligibleApplicant;

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
            var request = EligibilityTestScenarios.DisabledEligibleApplicant;

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
            var request = EligibilityTestScenarios.PregnantEligibleApplicant;

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
            var request = EligibilityTestScenarios.BlindEligibleApplicant;

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
            var request = EligibilityTestScenarios.OverIncomeApplicant;

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
        public async Task DetermineEligibility_WithInvalidAge_Returns422UnprocessableEntity()
        {
            // Arrange
            var request = EligibilityTestScenarios.InvalidAgeApplicant;

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            var problemDetails = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();

            Assert.NotNull(problemDetails);
            Assert.Equal(422, problemDetails.Status);
            Assert.Equal("Invalid Eligibility Data", problemDetails.Title);
        }

        [Fact]
        public async Task DetermineEligibility_WithInvalidHouseholdSize_Returns422UnprocessableEntity()
        {
            // Arrange
            var request = EligibilityTestScenarios.InvalidHouseholdSizeApplicant;

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            var problemDetails = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();

            Assert.NotNull(problemDetails);
            Assert.Equal(422, problemDetails.Status);
            Assert.Equal("Invalid Eligibility Data", problemDetails.Title);
        }

        [Fact]
        public async Task DetermineEligibility_WithNegativeIncome_Returns422UnprocessableEntity()
        {
            // Arrange
            var request = EligibilityTestScenarios.NegativeIncomeApplicant;

            // Act
            var response = await _client.PostAsJsonAsync("/api/eligibility/determine", request);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
            var problemDetails = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();

            Assert.NotNull(problemDetails);
            Assert.Equal(422, problemDetails.Status);
            Assert.Equal("Invalid Eligibility Data", problemDetails.Title);
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

        [Fact]
        public async Task DetermineEligibility_WithNullBody_Returns400BadRequest()
        {
            // Arrange - Send explicit null JSON
            var nullContent = new StringContent(
                "null",
                System.Text.Encoding.UTF8,
                "application/json");

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", nullContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
            var problemDetails = await response.Content.ReadFromJsonAsync<Microsoft.AspNetCore.Mvc.ProblemDetails>();

            Assert.NotNull(problemDetails);
            Assert.Equal(400, problemDetails.Status);
        }

        #endregion

        #region Edge Cases

        [Fact]
        public async Task DetermineEligibility_WithMedicareAtExactIncomeLimit_ReturnsEligibleResult()
        {
            // Arrange
            var request = EligibilityTestScenarios.MedicareAtExactIncomeLimitApplicant;

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
            var request = EligibilityTestScenarios.MedicareOneDollarOverLimitApplicant;

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
            var request = EligibilityTestScenarios.Age65BoundaryApplicant;

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
            var request = EligibilityTestScenarios.Age64BoundaryApplicant;

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

        #region Health Check Tests

        [Fact]
        public async Task HealthCheck_ServiceAvailable_Returns200Ok()
        {
            // Act
            var response = await _client.GetAsync("/api/eligibility/health");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var healthStatus = await response.Content.ReadAsStringAsync();

            Assert.Contains("healthy", healthStatus);
            Assert.Contains("EligibilityService", healthStatus);
        }

        #endregion
    }
}
