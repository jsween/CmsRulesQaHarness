namespace CmsRulesQaHarness.Tests.TestData
{
    /// <summary>
    /// XML sample data for eligibility requests covering various scenarios.
    /// </summary>
    public static class XmlEligibilitySamples
    {
        /// <summary>
        /// Medicare-eligible applicant (age 65+, income within threshold)
        /// </summary>
        public static string MedicareEligible => @"
<EligibilityRequest>
  <ApplicantId>APP-1001</ApplicantId>
  <Age>67</Age>
  <MonthlyIncome>1200</MonthlyIncome>
  <HouseholdSize>1</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>false</Blind>
  <Pregnant>false</Pregnant>
  <Resources>1500</Resources>
  <ProgramType>ClassicMedicaid</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Disabled applicant eligible for Medicaid
        /// </summary>
        public static string DisabledEligible => @"
<EligibilityRequest>
  <ApplicantId>APP-1002</ApplicantId>
  <Age>45</Age>
  <MonthlyIncome>1500</MonthlyIncome>
  <HouseholdSize>2</HouseholdSize>
  <Disabled>true</Disabled>
  <Blind>false</Blind>
  <Pregnant>false</Pregnant>
  <Resources>2000</Resources>
  <ProgramType>ClassicMedicaid</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Pregnant applicant eligible for CHIP
        /// </summary>
        public static string PregnantEligible => @"
<EligibilityRequest>
  <ApplicantId>APP-1003</ApplicantId>
  <Age>28</Age>
  <MonthlyIncome>1800</MonthlyIncome>
  <HouseholdSize>3</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>false</Blind>
  <Pregnant>true</Pregnant>
  <Resources>1000</Resources>
  <ProgramType>CHIP</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Blind applicant eligible for Medicaid
        /// </summary>
        public static string BlindEligible => @"
<EligibilityRequest>
  <ApplicantId>APP-1004</ApplicantId>
  <Age>52</Age>
  <MonthlyIncome>2100</MonthlyIncome>
  <HouseholdSize>1</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>true</Blind>
  <Pregnant>false</Pregnant>
  <Resources>3000</Resources>
  <ProgramType>ClassicMedicaid</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Applicant with income too high (not eligible)
        /// </summary>
        public static string OverIncome => @"
<EligibilityRequest>
  <ApplicantId>APP-2001</ApplicantId>
  <Age>67</Age>
  <MonthlyIncome>3500</MonthlyIncome>
  <HouseholdSize>1</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>false</Blind>
  <Pregnant>false</Pregnant>
  <Resources>5000</Resources>
  <ProgramType>ClassicMedicaid</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Invalid request - negative age
        /// </summary>
        public static string InvalidAge => @"
<EligibilityRequest>
  <ApplicantId>APP-3001</ApplicantId>
  <Age>-5</Age>
  <MonthlyIncome>1000</MonthlyIncome>
  <HouseholdSize>1</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>false</Blind>
  <Pregnant>false</Pregnant>
  <Resources>500</Resources>
  <ProgramType>ClassicMedicaid</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Invalid request - zero household size
        /// </summary>
        public static string InvalidHouseholdSize => @"
<EligibilityRequest>
  <ApplicantId>APP-3002</ApplicantId>
  <Age>30</Age>
  <MonthlyIncome>1000</MonthlyIncome>
  <HouseholdSize>0</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>false</Blind>
  <Pregnant>false</Pregnant>
  <Resources>500</Resources>
  <ProgramType>ClassicMedicaid</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Invalid request - negative monthly income
        /// </summary>
        public static string NegativeIncome => @"
<EligibilityRequest>
  <ApplicantId>APP-3003</ApplicantId>
  <Age>40</Age>
  <MonthlyIncome>-500</MonthlyIncome>
  <HouseholdSize>2</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>false</Blind>
  <Pregnant>false</Pregnant>
  <Resources>1000</Resources>
  <ProgramType>ClassicMedicaid</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Edge case - exactly at Medicare income threshold
        /// </summary>
        public static string MedicareAtThreshold => @"
<EligibilityRequest>
  <ApplicantId>APP-4001</ApplicantId>
  <Age>65</Age>
  <MonthlyIncome>1500</MonthlyIncome>
  <HouseholdSize>1</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>false</Blind>
  <Pregnant>false</Pregnant>
  <Resources>2000</Resources>
  <ProgramType>Medicare</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Edge case - one dollar over Medicare income threshold
        /// </summary>
        public static string MedicareOverThreshold => @"
<EligibilityRequest>
  <ApplicantId>APP-4002</ApplicantId>
  <Age>66</Age>
  <MonthlyIncome>1501</MonthlyIncome>
  <HouseholdSize>1</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>false</Blind>
  <Pregnant>false</Pregnant>
  <Resources>2500</Resources>
  <ProgramType>Medicare</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Edge case - age 64 (just below Medicare eligibility)
        /// </summary>
        public static string Age64BelowMedicare => @"
<EligibilityRequest>
  <ApplicantId>APP-4003</ApplicantId>
  <Age>64</Age>
  <MonthlyIncome>1200</MonthlyIncome>
  <HouseholdSize>1</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>false</Blind>
  <Pregnant>false</Pregnant>
  <Resources>1500</Resources>
  <ProgramType>Medicare</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Complex case - multiple conditions (aged and disabled)
        /// </summary>
        public static string AgedAndDisabled => @"
<EligibilityRequest>
  <ApplicantId>APP-5001</ApplicantId>
  <Age>70</Age>
  <MonthlyIncome>1400</MonthlyIncome>
  <HouseholdSize>1</HouseholdSize>
  <Disabled>true</Disabled>
  <Blind>false</Blind>
  <Pregnant>false</Pregnant>
  <Resources>2500</Resources>
  <ProgramType>DualEligible</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Complex case - disabled and blind
        /// </summary>
        public static string DisabledAndBlind => @"
<EligibilityRequest>
  <ApplicantId>APP-5002</ApplicantId>
  <Age>50</Age>
  <MonthlyIncome>1700</MonthlyIncome>
  <HouseholdSize>1</HouseholdSize>
  <Disabled>true</Disabled>
  <Blind>true</Blind>
  <Pregnant>false</Pregnant>
  <Resources>2000</Resources>
  <ProgramType>ClassicMedicaid</ProgramType>
</EligibilityRequest>";

        /// <summary>
        /// Large household scenario
        /// </summary>
        public static string LargeHousehold => @"
<EligibilityRequest>
  <ApplicantId>APP-6001</ApplicantId>
  <Age>35</Age>
  <MonthlyIncome>2500</MonthlyIncome>
  <HouseholdSize>6</HouseholdSize>
  <Disabled>false</Disabled>
  <Blind>false</Blind>
  <Pregnant>true</Pregnant>
  <Resources>3000</Resources>
  <ProgramType>CHIP</ProgramType>
</EligibilityRequest>";
    }
}
