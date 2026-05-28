using CmsRulesQaHarness.API.Data;
using Microsoft.EntityFrameworkCore;

namespace CmsRulesQaHarness.Tests.Database;

public class EligibilityDatabaseTests
{
    [Fact]
    public async Task SeededDatabase_ShouldContainEligibilityTestCases()
    {
        var options = new DbContextOptionsBuilder<EligibilityDbContext>()
            .UseSqlite("Data Source=eligibility-test.db")
            .Options;

        await using var context = new EligibilityDbContext(options);

        await SeedData.InitializeAsync(context);

        var testCaseCount = await context.EligibilityTestCases.CountAsync();
        var applicantCount = await context.Applicants.CountAsync();
        var householdCount = await context.Households.CountAsync();

        Assert.Equal(8, testCaseCount);
        Assert.Equal(8, applicantCount);
        Assert.Equal(8, householdCount);
    }
}