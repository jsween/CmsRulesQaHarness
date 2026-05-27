namespace CmsRulesQaHarness.API.Models
{
    public class IncomeRecord
    {
        public int IncomeRecordId { get; set; }

        public int ApplicantId { get; set; }

        public Applicant? Applicant { get; set; }

        public string IncomeType { get; set; } = string.Empty;

        public decimal MonthlyAmount { get; set; }

        public bool IsCountable { get; set; } = true;
    }
}