using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using Tribe.Bib.Models.TribeRelated;

// === CREATOR DEFINITION ===
public class CreatorPlan
{
    [Key]
    public string Guid { get; set; } = System.Guid.NewGuid().ToString(); // System.Guid verwenden
    public string Name { get; set; } = string.Empty;
    public int TokenMenge { get; set; }
    public string FeaturesJson { get; set; } = "{}";
    public bool CanUploadDigitalContent { get; set; } = true;
    public bool HaveShopItems { get; set; } = true;
    public bool CanCreateEvents { get; set; } = true;
    public bool CanCreateRaffles { get; set; } = true;
    public bool CanUseWindowsApp { get; set; } = true;
    public bool Aktiv { get; set; }

    // Navigation Property für bessere EF Core Integration
    public virtual ICollection<CreatorPlanPricing> PricingList { get; set; } = new List<CreatorPlanPricing>();
}

public class CreatorPlanPricing
{
    [Key]
    public string Guid { get; set; } = System.Guid.NewGuid().ToString();

    [ForeignKey(nameof(CreatorPlan))]
    public string CreatorPlanGuid { get; set; }

    public string Duration { get; set; } = "Monthly";

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValueUSD { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValueEuro { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal ValueGbPound { get; set; }

    // Navigation Property
    public virtual CreatorPlan CreatorPlan { get; set; }
}

