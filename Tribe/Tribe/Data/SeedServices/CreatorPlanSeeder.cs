using Microsoft.EntityFrameworkCore;
using Tribe.Bib.Models.TribeRelated;

public static class CreatorPlanSeeder
{
    public static void SeedData(ModelBuilder modelBuilder)
    {
        var plans = GetCreatorPlans();
        var pricing = GetCreatorPlanPricing(plans);

        modelBuilder.Entity<CreatorPlan>().HasData(plans);
        modelBuilder.Entity<CreatorPlanPricing>().HasData(pricing);
    }

    private static List<CreatorPlan> GetCreatorPlans()
    {
        return new List<CreatorPlan>
        {
            new CreatorPlan
            {
                Guid = "basic-plan-guid-001",
                Name = "Basic Plan",
                TokenMenge = 10000,
                FeaturesJson = "{\"maxProjects\":1,\"support\":\"email\"}",
                Aktiv = true
            },
            new CreatorPlan
            {
                Guid = "pro-plan-guid-002",
                Name = "Pro Plan",
                TokenMenge = 50000,
                FeaturesJson = "{\"maxProjects\":10,\"support\":\"priority\"}",
                Aktiv = true
            }
        };
    }

    private static List<CreatorPlanPricing> GetCreatorPlanPricing(List<CreatorPlan> plans)
    {
        var pricing = new List<CreatorPlanPricing>();

        foreach (var plan in plans)
        {
            // Monthly pricing
            pricing.Add(new CreatorPlanPricing
            {
                Guid = $"{plan.Guid}-monthly",
                CreatorPlanGuid = plan.Guid,
                Duration = "Monthly",
                ValueUSD = plan.Name == "Basic Plan" ? 9.99m : 29.99m,
                ValueEuro = plan.Name == "Basic Plan" ? 8.99m : 26.99m,
                ValueGbPound = plan.Name == "Basic Plan" ? 7.99m : 23.99m
            });

            // Annual pricing
            pricing.Add(new CreatorPlanPricing
            {
                Guid = $"{plan.Guid}-annual",
                CreatorPlanGuid = plan.Guid,
                Duration = "Annual",
                ValueUSD = plan.Name == "Basic Plan" ? 99.99m : 299.99m,
                ValueEuro = plan.Name == "Basic Plan" ? 89.99m : 269.99m,
                ValueGbPound = plan.Name == "Basic Plan" ? 79.99m : 239.99m
            });
        }

        return pricing;
    }
}