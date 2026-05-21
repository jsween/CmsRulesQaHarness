using CmsRulesQaHarness.API.Models;
using CmsRulesQaHarness.API.Services;

using Microsoft.AspNetCore.Mvc;

namespace CmsRulesQaHarness.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EligibilityController(IEligibilityService eligibilityService) : ControllerBase
    {
        private readonly IEligibilityService _eligibilityService = eligibilityService;

        [HttpPost("determine")]
        public ActionResult<EligibilityResult> DetermineEligibility(EligibilityRequest request)
        {
            var result = _eligibilityService.DetermineEligibility(request);
            return Ok(result);
        }
    }
}
