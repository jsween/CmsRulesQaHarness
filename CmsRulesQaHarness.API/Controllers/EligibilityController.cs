using CmsRulesQaHarness.API.Models;
using CmsRulesQaHarness.API.Models.Enums;
using CmsRulesQaHarness.API.Services;

using Microsoft.AspNetCore.Mvc;

namespace CmsRulesQaHarness.API.Controllers
{
    /// <summary>
    /// Controller for eligibility determination operations.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EligibilityController(IEligibilityService eligibilityService) : ControllerBase
    {
        private readonly IEligibilityService _eligibilityService = eligibilityService;

        /// <summary>
        /// Determines eligibility for CMS programs based on applicant information.
        /// </summary>
        /// <param name="request">The eligibility request containing applicant details.</param>
        /// <returns>
        /// 200 OK - Successfully determined eligibility
        /// 400 Bad Request - Invalid input data (negative age, invalid household size, etc.)
        /// 422 Unprocessable Entity - Valid format but business rule validation failed
        /// </returns>
        [HttpPost("determine")]
        [ProducesResponseType(typeof(EligibilityResult), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status422UnprocessableEntity)]
        public ActionResult<EligibilityResult> DetermineEligibility(EligibilityRequest request)
        {
            // 400 Bad Request - Null or missing required data
            if (request == null)
            {
                return BadRequest(new ProblemDetails
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Invalid Request",
                    Detail = "Request body cannot be null or empty."
                });
            }

            // Call service to determine eligibility
            var result = _eligibilityService.DetermineEligibility(request);

            // 422 Unprocessable Entity - Invalid business data (negative values, zero household, etc.)
            if (result.ProgramCategory == ProgramCategory.Invalid)
            {
                return UnprocessableEntity(new ProblemDetails
                {
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Title = "Invalid Eligibility Data",
                    Detail = "The request contains invalid data that cannot be processed.",
                    Extensions = { ["reasons"] = result.Reasons }
                });
            }

            // 200 OK - Successfully determined eligibility (eligible or not eligible)
            return Ok(result);
        }

        /// <summary>
        /// Health check endpoint to verify service availability.
        /// </summary>
        /// <returns>
        /// 200 OK - Service is available
        /// 503 Service Unavailable - Service is down or experiencing issues
        /// </returns>
        [HttpGet("health")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status503ServiceUnavailable)]
        public ActionResult HealthCheck()
        {
            try
            {
                // Simple check - if service is injected and accessible, we're healthy
                if (_eligibilityService == null)
                {
                    return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
                    {
                        Status = StatusCodes.Status503ServiceUnavailable,
                        Title = "Service Unavailable",
                        Detail = "Eligibility service is not available."
                    });
                }

                return Ok(new { status = "healthy", service = "EligibilityService", timestamp = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                // 503 Service Unavailable - Unexpected error
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new ProblemDetails
                {
                    Status = StatusCodes.Status503ServiceUnavailable,
                    Title = "Service Unavailable",
                    Detail = $"Service health check failed: {ex.Message}"
                });
            }
        }
    }
}
