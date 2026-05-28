using Microsoft.EntityFrameworkCore;
using CmsRulesQaHarness.API.Models.Entities;

namespace CmsRulesQaHarness.API.Data
{
    public class EligibilityDbContext(DbContextOptions<EligibilityDbContext> options) : DbContext(options)
    {
        public DbSet<Household> Households => Set<Household>();

        public DbSet<Applicant> Applicants => Set<Applicant>();

        public DbSet<IncomeRecord> IncomeRecords => Set<IncomeRecord>();

        public DbSet<EligibilityTestCase> EligibilityTestCases => Set<EligibilityTestCase>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Household>(entity =>
            {
                entity.HasKey(h => h.HouseholdId);

                entity.Property(h => h.CaseNumber)
                .IsRequired()
                .HasMaxLength(25);

                entity.Property(h => h.State)
                    .IsRequired()
                    .HasMaxLength(2);

                entity.Property(h => h.County)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(h => h.Applicants)
                    .WithOne(a => a.Household)
                    .HasForeignKey(a => a.HouseholdId);
            });

            modelBuilder.Entity<Applicant>(entity =>
            {
                entity.HasKey(a => a.ApplicantId);

                entity.Property(a => a.FirstName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(a => a.LastName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasMany(a => a.IncomeRecords)
                    .WithOne(i => i.Applicant)
                    .HasForeignKey(i => i.ApplicantId);

                entity.HasMany(a => a.EligibilityTestCases)
                    .WithOne(t => t.Applicant)
                    .HasForeignKey(t => t.ApplicantId);
            });

            modelBuilder.Entity<IncomeRecord>(entity =>
            {
                entity.HasKey(i => i.IncomeRecordId);

                entity.Property(i => i.IncomeType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(i => i.MonthlyAmount)
                    .HasPrecision(10, 2);
            });

            modelBuilder.Entity<EligibilityTestCase>(entity =>
            {
                entity.HasKey(t => t.EligibilityTestCaseId);

                entity.Property(t => t.TestCaseNumber)
                    .IsRequired()
                    .HasMaxLength(25);

                entity.Property(t => t.ScenarioName)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(t => t.ProgramType)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(t => t.IncomeLimit)
                    .HasPrecision(10, 2);

                entity.Property(t => t.ExpectedReason)
                    .IsRequired()
                    .HasMaxLength(500);
            });
        }
    }
}
