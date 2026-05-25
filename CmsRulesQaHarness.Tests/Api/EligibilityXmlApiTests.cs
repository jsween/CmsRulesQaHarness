using CmsRulesQaHarness.API.Models;
using CmsRulesQaHarness.API.Models.Enums;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Xml.Linq;

namespace CmsRulesQaHarness.Tests.Api
{
    /// <summary>
    /// Tests the API layer's XML input deserialization.
    /// These tests verify that the API correctly accepts and processes XML format requests.
    /// </summary>
    public class EligibilityXmlApiTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        private static StringContent CreateXmlContent(string xmlContent)
        {
            return new StringContent(xmlContent, Encoding.UTF8, "application/xml");
        }

        #region Positive Cases - XML Input

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithValidMedicareApplicant_ReturnsEligibleResult()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>67</Age>
  <MonthlyIncome>1200</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicare, result.ProgramCategory);
            Assert.Contains(ValidationReasons.Age65OrOlderIncomeThreshold, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithDisabledApplicant_ReturnsEligibleMedicaidResult()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>42</Age>
  <MonthlyIncome>1700</MonthlyIncome>
  <IsDisabled>true</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicaid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.DisabledIncomeThreshold, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithPregnantApplicant_ReturnsEligibleCHIPResult()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>28</Age>
  <MonthlyIncome>1900</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>true</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>2</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.CHIP, result.ProgramCategory);
            Assert.Contains(ValidationReasons.PregnantIncomeThreshold, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithBlindApplicant_ReturnsEligibleMedicaidResult()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>35</Age>
  <MonthlyIncome>1500</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>true</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicaid, result.ProgramCategory);
            Assert.Contains(ValidationReasons.BlindIncomeThreshold, result.Reasons);
        }

        #endregion

        #region Negative Cases - XML Format

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithOverIncomeApplicant_ReturnsNotEligibleResult()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>67</Age>
  <MonthlyIncome>5000</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
            Assert.Contains(ValidationReasons.NotEligible, result.Reasons);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithInvalidAge_ReturnsInvalidResult()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>-1</Age>
  <MonthlyIncome>1000</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert - Invalid data returns 422 Unprocessable Entity
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithInvalidHouseholdSize_ReturnsInvalidResult()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>45</Age>
  <MonthlyIncome>1500</MonthlyIncome>
  <IsDisabled>true</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>0</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert - Invalid data returns 422 Unprocessable Entity
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithNegativeIncome_ReturnsInvalidResult()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>40</Age>
  <MonthlyIncome>-500</MonthlyIncome>
  <IsDisabled>true</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert - Invalid data returns 422 Unprocessable Entity
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        #endregion

        #region Edge Cases - XML Format

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithMedicareAtExactIncomeLimit_ReturnsEligibleResult()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>70</Age>
  <MonthlyIncome>1600</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicare, result.ProgramCategory);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithMedicareOneDollarOverLimit_ReturnsNotEligible()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>70</Age>
  <MonthlyIncome>1601</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithAge65Boundary_ReturnsEligibleMedicare()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>65</Age>
  <MonthlyIncome>1500</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.True(result.IsEligible);
            Assert.Equal(ProgramCategory.Medicare, result.ProgramCategory);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithAge64_ReturnsNotEligibleForMedicare()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>64</Age>
  <MonthlyIncome>1500</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();

            Assert.NotNull(result);
            Assert.False(result.IsEligible);
            Assert.Equal(ProgramCategory.None, result.ProgramCategory);
        }

        #endregion

        #region XML Malformed/Error Cases

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithMalformedXml_ReturnsBadRequest()
        {
            // Arrange
            var malformedXml = @"
<EligibilityRequest>
  <Age>67</Age>
  <MonthlyIncome>1200
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(malformedXml));

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithMissingRequiredField_ReturnsBadRequestOrInvalid()
        {
            // Arrange - Missing Age field
            var incompleteXml = @"
<EligibilityRequest>
  <MonthlyIncome>1200</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(incompleteXml));

            // Assert - Either 400 BadRequest or 200 with Invalid result is acceptable
            Assert.True(
                response.StatusCode == HttpStatusCode.BadRequest || 
                response.StatusCode == HttpStatusCode.OK,
                $"Expected BadRequest or OK, got {response.StatusCode}");
        }

        #endregion

        #region XML Input Acceptance Tests

        [Fact]
        public async Task DetermineEligibility_XmlRequest_IsAcceptedSuccessfully()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>67</Age>
  <MonthlyIncome>1200</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            // Act
            var response = await _client.PostAsync("/api/eligibility/determine", CreateXmlContent(xmlRequest));

            // Assert - Verify XML input is accepted and processed
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<EligibilityResult>();
            Assert.NotNull(result);
            Assert.True(result.IsEligible);
        }

        [Fact]
        public async Task DetermineEligibility_XmlRequest_WithAcceptXmlHeader_ReturnsXmlResponse()
        {
            // Arrange
            var xmlRequest = @"
<EligibilityRequest>
  <Age>67</Age>
  <MonthlyIncome>1200</MonthlyIncome>
  <IsDisabled>false</IsDisabled>
  <IsPregnant>false</IsPregnant>
  <IsBlind>false</IsBlind>
  <HouseholdSize>1</HouseholdSize>
</EligibilityRequest>";

            var request = new HttpRequestMessage(HttpMethod.Post, "/api/eligibility/determine")
            {
                Content = CreateXmlContent(xmlRequest)
            };
            request.Headers.Add("Accept", "application/xml");

            // Act
            var response = await _client.SendAsync(request);

            // Assert - When Accept header is XML, response should be XML
            response.EnsureSuccessStatusCode();
            Assert.Equal("application/xml", response.Content.Headers.ContentType?.MediaType);

            var content = await response.Content.ReadAsStringAsync();
            Assert.StartsWith("<", content.TrimStart());
            Assert.Contains("EligibilityResult", content);
        }

        #endregion
    }
}
