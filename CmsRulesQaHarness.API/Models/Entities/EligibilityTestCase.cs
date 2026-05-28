namespace CmsRulesQaHarness.API.Models.Entities
{
    public class EligibilityTestCase
    {
        public int EligibilityTestCaseId { get; set; }

        public string TestCaseNumber { get; set; } = string.Empty;

        public string ScenarioName { get; set; } = string.Empty;

        public int ApplicantId { get; set; }

        public Applicant? Applicant { get; set; }

        public string ProgramType { get; set; } = string.Empty;

        public decimal IncomeLimit { get; set; }

        public bool ExpectedIsEligible { get; set; }

        public string ExpectedReason { get; set; } = string.Empty;
    }
}
