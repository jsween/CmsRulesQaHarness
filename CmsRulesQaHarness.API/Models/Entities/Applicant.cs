namespace CmsRulesQaHarness.API.Models.Entities
{
    public class Applicant
    {
        public int ApplicantId { get; set; }

        public int HouseholdId { get; set; }

        public Household? Household { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateOnly DateOfBirth { get; set; }

        public bool IsDisabled { get; set; }

        public bool IsPregnant { get; set; }

        public bool IsBlind { get; set; }

        public bool IsReceivingSsi { get; set; }

        public List<IncomeRecord> IncomeRecords { get; set; } = new();

        public List<EligibilityTestCase> EligibilityTestCases { get; set; } = new();
    }
}
