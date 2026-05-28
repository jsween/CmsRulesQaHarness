using CmsRulesQaHarness.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CmsRulesQaHarness.API.Data;

public static class SeedData
{
    public static async Task InitializeAsync(EligibilityDbContext context)
    {
        if (await context.Households.AnyAsync())
        {
            return;
        }

        var households = new List<Household>
        {
            new()
            {
                HouseholdId = 1,
                CaseNumber = "CASE-001",
                HouseholdSize = 1,
                State = "WA",
                County = "Clark",
                Applicants =
                [
                    new()
                    {
                        ApplicantId = 1,
                        FirstName = "Mary",
                        LastName = "Senior",
                        DateOfBirth = new DateOnly(1954, 5, 15),
                        IsDisabled = false,
                        IsPregnant = false,
                        IsBlind = false,
                        IsReceivingSsi = false,
                        IncomeRecords =
                        [
                            new()
                            {
                                IncomeRecordId = 1,
                                IncomeType = "SocialSecurity",
                                MonthlyAmount = 1200.00m,
                                IsCountable = true
                            }
                        ],
                        EligibilityTestCases =
                        [
                            new()
                            {
                                EligibilityTestCaseId = 1,
                                TestCaseNumber = "TC-001",
                                ScenarioName = "Senior applicant under income limit",
                                ProgramType = "Classic Medicaid",
                                IncomeLimit = 1600.00m,
                                ExpectedIsEligible = true,
                                ExpectedReason = "Applicant is age 65 or older and under the income limit."
                            }
                        ]
                    }
                ]
            },

            new()
            {
                HouseholdId = 2,
                CaseNumber = "CASE-002",
                HouseholdSize = 1,
                State = "WA",
                County = "Clark",
                Applicants =
                [
                    new()
                    {
                        ApplicantId = 2,
                        FirstName = "Robert",
                        LastName = "OverIncome",
                        DateOfBirth = new DateOnly(1950, 8, 20),
                        IncomeRecords =
                        [
                            new()
                            {
                                IncomeRecordId = 2,
                                IncomeType = "Pension",
                                MonthlyAmount = 1601.00m,
                                IsCountable = true
                            }
                        ],
                        EligibilityTestCases =
                        [ 
                            new()
                            {
                                EligibilityTestCaseId = 2,
                                TestCaseNumber = "TC-002",
                                ScenarioName = "Senior applicant one dollar over income limit",
                                ProgramType = "Classic Medicaid",
                                IncomeLimit = 1600.00m,
                                ExpectedIsEligible = false,
                                ExpectedReason = "Applicant is age 65 or older but exceeds the income limit."
                            }
                        ]
                    }
                ]
            },

            new()
            {
                HouseholdId = 3,
                CaseNumber = "CASE-003",
                HouseholdSize = 1,
                State = "WA",
                County = "Pierce",
                Applicants =
                [
                    new()
                    {
                        ApplicantId = 3,
                        FirstName = "Diane",
                        LastName = "Disabled",
                        DateOfBirth = new DateOnly(1980, 3, 12),
                        IsDisabled = true,
                        IncomeRecords =
                        [
                            new()
                            {
                                IncomeRecordId = 3,
                                IncomeType = "Wages",
                                MonthlyAmount = 1500.00m,
                                IsCountable = true
                            }
                        ],
                        EligibilityTestCases =
                        [
                            new()
                            {
                                EligibilityTestCaseId = 3,
                                TestCaseNumber = "TC-003",
                                ScenarioName = "Disabled applicant under income limit",
                                ProgramType = "Classic Medicaid",
                                IncomeLimit = 1600.00m,
                                ExpectedIsEligible = true,
                                ExpectedReason = "Applicant is disabled and under the income limit."
                            }
                        ]
                    }
                ]
            },

            new()
            {
                HouseholdId = 4,
                CaseNumber = "CASE-004",
                HouseholdSize = 2,
                State = "WA",
                County = "King",
                Applicants =
                [
                    new()
                    {
                        ApplicantId = 4,
                        FirstName = "Paula",
                        LastName = "Pregnant",
                        DateOfBirth = new DateOnly(1995, 9, 2),
                        IsPregnant = true,
                        IncomeRecords =
                        [
                            new()
                            {
                                IncomeRecordId = 4,
                                IncomeType = "Wages",
                                MonthlyAmount = 1800.00m,
                                IsCountable = true
                            }
                        ],
                        EligibilityTestCases =
                        [
                            new()
                            {
                                EligibilityTestCaseId = 4,
                                TestCaseNumber = "TC-004",
                                ScenarioName = "Pregnant applicant under pregnancy income threshold",
                                ProgramType = "Pregnancy Medical",
                                IncomeLimit = 2000.00m,
                                ExpectedIsEligible = true,
                                ExpectedReason = "Applicant is pregnant and under the pregnancy income limit."
                            }
                        ]
                    }
                ]
            },

            new()
            {
                HouseholdId = 5,
                CaseNumber = "CASE-005",
                HouseholdSize = 1,
                State = "WA",
                County = "Spokane",
                Applicants =
                [
                    new()
                    {
                        ApplicantId = 5,
                        FirstName = "Ben",
                        LastName = "Blind",
                        DateOfBirth = new DateOnly(1975, 1, 10),
                        IsBlind = true,
                        IncomeRecords =
                        [
                            new()
                            {
                                IncomeRecordId = 5,
                                IncomeType = "Wages",
                                MonthlyAmount = 2200.00m,
                                IsCountable = true
                            }
                        ],
                        EligibilityTestCases =
                        [
                            new()
                            {
                                EligibilityTestCaseId = 5,
                                TestCaseNumber = "TC-005",
                                ScenarioName = "Blind applicant at income limit",
                                ProgramType = "Classic Medicaid",
                                IncomeLimit = 2200.00m,
                                ExpectedIsEligible = true,
                                ExpectedReason = "Applicant is blind and at the income limit."
                            }
                        ]
                    }
                ]
            },

            new()
            {
                HouseholdId = 6,
                CaseNumber = "CASE-006",
                HouseholdSize = 1,
                State = "WA",
                County = "Spokane",
                Applicants =
                [
                    new()
                    {
                        ApplicantId = 6,
                        FirstName = "Brenda",
                        LastName = "BlindOverLimit",
                        DateOfBirth = new DateOnly(1970, 11, 25),
                        IsBlind = true,
                        IncomeRecords =
                        [
                            new()
                            {
                                IncomeRecordId = 6,
                                IncomeType = "Wages",
                                MonthlyAmount = 2201.00m,
                                IsCountable = true
                            }
                        ],
                        EligibilityTestCases =
                        [
                            new()
                            {
                                EligibilityTestCaseId = 6,
                                TestCaseNumber = "TC-006",
                                ScenarioName = "Blind applicant one dollar over income limit",
                                ProgramType = "Classic Medicaid",
                                IncomeLimit = 2200.00m,
                                ExpectedIsEligible = false,
                                ExpectedReason = "Applicant is blind but exceeds the income limit."
                            }
                        ]
                    }
                ]
            },

            new()
            {
                HouseholdId = 7,
                CaseNumber = "CASE-007",
                HouseholdSize = 1,
                State = "WA",
                County = "Clark",
                Applicants =
                [
                    new()
                    {
                        ApplicantId = 7,
                        FirstName = "Nora",
                        LastName = "NegativeIncome",
                        DateOfBirth = new DateOnly(1990, 7, 7),
                        IncomeRecords =
                        [
                            new()
                            {
                                IncomeRecordId = 7,
                                IncomeType = "Wages",
                                MonthlyAmount = -100.00m,
                                IsCountable = true
                            }
                        ],
                        EligibilityTestCases =
                        [
                            new()
                            {
                                EligibilityTestCaseId = 7,
                                TestCaseNumber = "TC-007",
                                ScenarioName = "Invalid applicant with negative income",
                                ProgramType = "Validation",
                                IncomeLimit = 0.00m,
                                ExpectedIsEligible = false,
                                ExpectedReason = "Negative income should fail validation."
                            }
                        ]
                    }
                ]
            },

            new()
            {
                HouseholdId = 8,
                CaseNumber = "CASE-008",
                HouseholdSize = 0,
                State = "WA",
                County = "Clark",
                Applicants =
                [
                    new()
                    {
                        ApplicantId = 8,
                        FirstName = "Henry",
                        LastName = "InvalidHousehold",
                        DateOfBirth = new DateOnly(1992, 4, 18),
                        IncomeRecords =
                        [
                            new()
                            {
                                IncomeRecordId = 8,
                                IncomeType = "Wages",
                                MonthlyAmount = 500.00m,
                                IsCountable = true
                            }
                        ],
                        EligibilityTestCases =
                        [
                            new()
                            {
                                EligibilityTestCaseId = 8,
                                TestCaseNumber = "TC-008",
                                ScenarioName = "Invalid applicant with household size zero",
                                ProgramType = "Validation",
                                IncomeLimit = 0.00m,
                                ExpectedIsEligible = false,
                                ExpectedReason = "Household size of zero should fail validation."
                            }
                        ]
                    }
                ]
            }
        };

        context.Households.AddRange(households);
        await context.SaveChangesAsync();
    }
}