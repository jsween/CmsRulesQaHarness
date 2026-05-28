using CmsRulesQaHarness.API.Data;
using CmsRulesQaHarness.API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace CmsRulesQaHarness.Tests.Database;

public class EligibilityDatabaseTests : IAsyncLifetime
{
    private EligibilityDbContext _context = null!;
    private DbContextOptions<EligibilityDbContext> _options = null!;

    public async Task InitializeAsync()
    {
        // Use in-memory SQLite database for each test
        _options = new DbContextOptionsBuilder<EligibilityDbContext>()
            .UseSqlite("Data Source=:memory:")
            .Options;

        _context = new EligibilityDbContext(_options);
        await _context.Database.OpenConnectionAsync(); // Required for in-memory SQLite
        await _context.Database.EnsureCreatedAsync();
        await SeedData.InitializeAsync(_context);
    }

    public async Task DisposeAsync()
    {
        await _context.Database.CloseConnectionAsync();
        await _context.DisposeAsync();
    }

    #region Seeding and Count Tests

    [Fact]
    public async Task SeededDatabase_ShouldContainEligibilityTestCases()
    {
        // Act
        var testCaseCount = await _context.EligibilityTestCases.CountAsync();
        var applicantCount = await _context.Applicants.CountAsync();
        var householdCount = await _context.Households.CountAsync();

        // Assert
        Assert.Equal(8, testCaseCount);
        Assert.Equal(8, applicantCount);
        Assert.Equal(8, householdCount);
    }

    [Fact]
    public async Task SeededDatabase_ShouldContainIncomeRecords()
    {
        // Act
        var incomeRecordCount = await _context.IncomeRecords.CountAsync();

        // Assert
        Assert.Equal(8, incomeRecordCount);
    }

    #endregion

    #region Navigation Property Tests

    [Fact]
    public async Task Household_ShouldHaveApplicants()
    {
        // Act
        var household = await _context.Households
            .Include(h => h.Applicants)
            .FirstOrDefaultAsync(h => h.HouseholdId == 1);

        // Assert
        Assert.NotNull(household);
        Assert.NotEmpty(household.Applicants);
        Assert.Single(household.Applicants);
        Assert.Equal("Mary", household.Applicants.First().FirstName);
    }

    [Fact]
    public async Task Applicant_ShouldHaveIncomeRecords()
    {
        // Act
        var applicant = await _context.Applicants
            .Include(a => a.IncomeRecords)
            .FirstOrDefaultAsync(a => a.ApplicantId == 1);

        // Assert
        Assert.NotNull(applicant);
        Assert.NotEmpty(applicant.IncomeRecords);
        Assert.Single(applicant.IncomeRecords);
        Assert.Equal(1200.00m, applicant.IncomeRecords.First().MonthlyAmount);
    }

    [Fact]
    public async Task Applicant_ShouldHaveEligibilityTestCases()
    {
        // Act
        var applicant = await _context.Applicants
            .Include(a => a.EligibilityTestCases)
            .FirstOrDefaultAsync(a => a.ApplicantId == 1);

        // Assert
        Assert.NotNull(applicant);
        Assert.NotEmpty(applicant.EligibilityTestCases);
        Assert.Single(applicant.EligibilityTestCases);
        Assert.Equal("TC-001", applicant.EligibilityTestCases.First().TestCaseNumber);
    }

    [Fact]
    public async Task IncomeRecord_ShouldHaveApplicantReference()
    {
        // Act
        var incomeRecord = await _context.IncomeRecords
            .Include(i => i.Applicant)
            .FirstOrDefaultAsync(i => i.IncomeRecordId == 1);

        // Assert
        Assert.NotNull(incomeRecord);
        Assert.NotNull(incomeRecord.Applicant);
        Assert.Equal("Mary", incomeRecord.Applicant.FirstName);
    }

    [Fact]
    public async Task EligibilityTestCase_ShouldHaveApplicantReference()
    {
        // Act
        var testCase = await _context.EligibilityTestCases
            .Include(t => t.Applicant)
            .FirstOrDefaultAsync(t => t.EligibilityTestCaseId == 1);

        // Assert
        Assert.NotNull(testCase);
        Assert.NotNull(testCase.Applicant);
        Assert.Equal("Mary", testCase.Applicant.FirstName);
    }

    #endregion

    #region Query Tests

    [Fact]
    public async Task QueryApplicantById_ShouldReturnCorrectApplicant()
    {
        // Act
        var applicant = await _context.Applicants.FindAsync(2);

        // Assert
        Assert.NotNull(applicant);
        Assert.Equal("Robert", applicant.FirstName);
        Assert.Equal("OverIncome", applicant.LastName);
    }

    [Fact]
    public async Task QueryApplicantsByDisabledStatus_ShouldReturnOnlyDisabled()
    {
        // Act
        var disabledApplicants = await _context.Applicants
            .Where(a => a.IsDisabled)
            .ToListAsync();

        // Assert
        Assert.Single(disabledApplicants);
        Assert.Equal("Diane", disabledApplicants.First().FirstName);
    }

    [Fact]
    public async Task QueryApplicantsByPregnantStatus_ShouldReturnOnlyPregnant()
    {
        // Act
        var pregnantApplicants = await _context.Applicants
            .Where(a => a.IsPregnant)
            .ToListAsync();

        // Assert
        Assert.Single(pregnantApplicants);
        Assert.Equal("Paula", pregnantApplicants.First().FirstName);
    }

    [Fact]
    public async Task QueryApplicantsByBlindStatus_ShouldReturnOnlyBlind()
    {
        // Act
        var blindApplicants = await _context.Applicants
            .Where(a => a.IsBlind)
            .ToListAsync();

        // Assert
        Assert.Equal(2, blindApplicants.Count);
        Assert.Contains(blindApplicants, a => a.FirstName == "Ben");
        Assert.Contains(blindApplicants, a => a.FirstName == "Brenda");
    }

    [Fact]
    public async Task QueryTestCasesByProgramType_ShouldReturnMatchingCases()
    {
        // Act
        var classicMedicaidCases = await _context.EligibilityTestCases
            .Where(t => t.ProgramType == "Classic Medicaid")
            .ToListAsync();

        // Assert
        Assert.Equal(5, classicMedicaidCases.Count);
    }

    [Fact]
    public async Task QueryTestCasesByExpectedEligibility_ShouldReturnMatchingCases()
    {
        // Act
        var eligibleCases = await _context.EligibilityTestCases
            .Where(t => t.ExpectedIsEligible)
            .ToListAsync();

        // Assert
        Assert.Equal(4, eligibleCases.Count);
    }

    [Fact]
    public async Task QueryHouseholdsByState_ShouldReturnAllWashington()
    {
        // Act
        var waHouseholds = await _context.Households
            .Where(h => h.State == "WA")
            .ToListAsync();

        // Assert
        Assert.Equal(8, waHouseholds.Count);
    }

    [Fact]
    public async Task QueryIncomeRecordsByType_ShouldReturnMatchingRecords()
    {
        // Act
        var wageRecords = await _context.IncomeRecords
            .Where(i => i.IncomeType == "Wages")
            .ToListAsync();

        // Assert
        Assert.Equal(6, wageRecords.Count);
    }

    #endregion

    #region Business Logic Query Tests

    [Fact]
    public async Task QueryApplicantsOverIncomeLimit_ShouldReturnCorrectApplicants()
    {
        // Arrange
        decimal incomeLimit = 1600.00m;

        // Act
        var applicantsOverLimit = await _context.Applicants
            .Include(a => a.IncomeRecords)
            .Where(a => a.IncomeRecords.Sum(i => i.MonthlyAmount) > incomeLimit)
            .ToListAsync();

        // Assert
        Assert.Equal(4, applicantsOverLimit.Count);
        Assert.Contains(applicantsOverLimit, a => a.FirstName == "Robert");
        Assert.Contains(applicantsOverLimit, a => a.FirstName == "Paula");
    }

    [Fact]
    public async Task QueryApplicantsWithNegativeIncome_ShouldReturnCorrectApplicants()
    {
        // Act
        var applicantsWithNegativeIncome = await _context.Applicants
            .Include(a => a.IncomeRecords)
            .Where(a => a.IncomeRecords.Any(i => i.MonthlyAmount < 0))
            .ToListAsync();

        // Assert
        Assert.Single(applicantsWithNegativeIncome);
        Assert.Equal("Nora", applicantsWithNegativeIncome.First().FirstName);
    }

    [Fact]
    public async Task QueryValidationTestCases_ShouldReturnCorrectCases()
    {
        // Act
        var validationCases = await _context.EligibilityTestCases
            .Where(t => t.ProgramType == "Validation")
            .ToListAsync();

        // Assert
        Assert.Equal(2, validationCases.Count);
    }

    [Fact]
    public async Task QueryHouseholdsWithInvalidSize_ShouldReturnCorrectHouseholds()
    {
        // Act
        var invalidHouseholds = await _context.Households
            .Where(h => h.HouseholdSize <= 0)
            .ToListAsync();

        // Assert
        Assert.Single(invalidHouseholds);
        Assert.Equal("CASE-008", invalidHouseholds.First().CaseNumber);
    }

    #endregion

    #region Complex Query Tests

    [Fact]
    public async Task QueryApplicantsWithFullDetails_ShouldIncludeAllRelatedData()
    {
        // Act
        var applicant = await _context.Applicants
            .Include(a => a.Household)
            .Include(a => a.IncomeRecords)
            .Include(a => a.EligibilityTestCases)
            .FirstOrDefaultAsync(a => a.ApplicantId == 1);

        // Assert
        Assert.NotNull(applicant);
        Assert.NotNull(applicant.Household);
        Assert.NotEmpty(applicant.IncomeRecords);
        Assert.NotEmpty(applicant.EligibilityTestCases);
        Assert.Equal("CASE-001", applicant.Household.CaseNumber);
    }

    [Fact]
    public async Task QueryHouseholdsWithAllApplicantsAndIncome_ShouldIncludeNestedData()
    {
        // Act
        var household = await _context.Households
            .Include(h => h.Applicants)
                .ThenInclude(a => a.IncomeRecords)
            .FirstOrDefaultAsync(h => h.HouseholdId == 1);

        // Assert
        Assert.NotNull(household);
        Assert.NotEmpty(household.Applicants);
        Assert.NotEmpty(household.Applicants.First().IncomeRecords);
    }

    [Fact]
    public async Task QueryTestCasesByIncomeThreshold_ShouldReturnCorrectCases()
    {
        // Act
        var highIncomeThresholdCases = await _context.EligibilityTestCases
            .Where(t => t.IncomeLimit >= 2000.00m)
            .ToListAsync();

        // Assert
        Assert.Equal(3, highIncomeThresholdCases.Count);
    }

    #endregion

    #region CRUD Operation Tests

    [Fact]
    public async Task CreateNewHousehold_ShouldPersistToDatabase()
    {
        // Arrange
        var newHousehold = new Household
        {
            CaseNumber = "CASE-999",
            HouseholdSize = 1,
            State = "WA",
            County = "Snohomish"
        };

        // Act
        _context.Households.Add(newHousehold);
        await _context.SaveChangesAsync();

        var retrieved = await _context.Households
            .FirstOrDefaultAsync(h => h.CaseNumber == "CASE-999");

        // Assert
        Assert.NotNull(retrieved);
        Assert.Equal("Snohomish", retrieved.County);
    }

    [Fact]
    public async Task UpdateApplicantIncome_ShouldPersistChanges()
    {
        // Arrange
        var applicant = await _context.Applicants
            .Include(a => a.IncomeRecords)
            .FirstAsync(a => a.ApplicantId == 1);

        // Act
        applicant.IncomeRecords.First().MonthlyAmount = 1500.00m;
        await _context.SaveChangesAsync();

        _context.Entry(applicant).State = EntityState.Detached;
        var retrieved = await _context.Applicants
            .Include(a => a.IncomeRecords)
            .FirstAsync(a => a.ApplicantId == 1);

        // Assert
        Assert.Equal(1500.00m, retrieved.IncomeRecords.First().MonthlyAmount);
    }

    [Fact]
    public async Task DeleteIncomeRecord_ShouldRemoveFromDatabase()
    {
        // Arrange
        var incomeRecord = await _context.IncomeRecords.FindAsync(1);
        Assert.NotNull(incomeRecord);

        // Act
        _context.IncomeRecords.Remove(incomeRecord);
        await _context.SaveChangesAsync();

        var retrieved = await _context.IncomeRecords.FindAsync(1);

        // Assert
        Assert.Null(retrieved);
    }

    #endregion

    #region Edge Case Tests

    [Fact]
    public async Task QueryNonExistentApplicant_ShouldReturnNull()
    {
        // Act
        var applicant = await _context.Applicants.FindAsync(999);

        // Assert
        Assert.Null(applicant);
    }

    [Fact]
    public async Task QueryApplicantsWithNoIncomeRecords_ShouldReturnEmptyCollection()
    {
        // Arrange - Create applicant without income
        var newApplicant = new Applicant
        {
            HouseholdId = 1,
            FirstName = "Test",
            LastName = "NoIncome",
            DateOfBirth = new DateOnly(1990, 1, 1)
        };
        _context.Applicants.Add(newApplicant);
        await _context.SaveChangesAsync();

        // Act
        var applicant = await _context.Applicants
            .Include(a => a.IncomeRecords)
            .FirstAsync(a => a.FirstName == "Test");

        // Assert
        Assert.Empty(applicant.IncomeRecords);
    }

    #endregion
}