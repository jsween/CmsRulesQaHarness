using CmsRulesQaHarness.API.Models;

namespace CmsRulesQaHarness.API.Services
{
    /// <summary>
    /// Defines the contract for determining CMS program eligibility.
    /// </summary>
    public interface IEligibilityService
    {
        /// <summary>
        /// Determines eligibility for CMS programs based on the provided request information.
        /// </summary>
        /// <param name="request">The eligibility request containing applicant information.</param>
        /// <returns>An <see cref="EligibilityResult"/> containing the determination outcome.</returns>
        EligibilityResult DetermineEligibility(EligibilityRequest request);
    }
}
