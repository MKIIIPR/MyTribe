using Microsoft.EntityFrameworkCore;
using static Tribe.Bib.ShopRelated.ShopStruckture;
using Tribe.Bib.ShopRelated;

namespace Tribe.Data
{
    public class ShopDbContext : DbContext
    {
        // Dies ist der einzige Konstruktor, der hier bleiben sollte.
        public ShopDbContext(DbContextOptions<ShopDbContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "name=ConnectionStrings:ShopDbConnection",
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)
                          .MigrationsHistoryTable("__EFMigrationsHistory", "application")
                );
            }
        }
        // Haupttabellen
        public DbSet<VideoProduct> VideoProducts { get; set; }
        public DbSet<ImageProduct> ImageProducts { get; set; }
        public DbSet<PhysicalProduct> PhysicalProducts { get; set; }
        public DbSet<ServiceProduct> ServiceProducts { get; set; }
        public DbSet<EventTicketProduct> EventTicketProducts { get; set; }

        public DbSet<ShopCategory> ShopCategories { get; set; }
        public DbSet<ShopOrder> ShopOrders { get; set; }
        public DbSet<ShopOrderItem> ShopOrderItems { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<ShopProduct> Products { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ... Der Rest Ihres OnModelCreating Codes bleibt unverändert ...

            // Table-per-Type (TPT) Konfiguration
            modelBuilder.Entity<ShopProduct>().ToTable("ShopProducts");
            modelBuilder.Entity<VideoProduct>().ToTable("VideoProducts");
            modelBuilder.Entity<ImageProduct>().ToTable("ImageProducts");
            modelBuilder.Entity<PhysicalProduct>().ToTable("PhysicalProducts");
            modelBuilder.Entity<ServiceProduct>().ToTable("ServiceProducts");
            modelBuilder.Entity<EventTicketProduct>().ToTable("EventTicketProducts");

            // JSON Konfiguration für Listen
            ConfigureJsonColumns(modelBuilder);

            // Indizes für Performance
            ConfigureIndexes(modelBuilder);

            // Fremdschlüssel Beziehungen
            ConfigureForeignKeys(modelBuilder);
        }

        private void ConfigureJsonColumns(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopProduct>()
                .Property(e => e.ImageUrls)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                );

            modelBuilder.Entity<ShopProduct>()
                .Property(e => e.Tags)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                );

            modelBuilder.Entity<VideoProduct>()
                .Property(e => e.SubtitleLanguages)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                );

            modelBuilder.Entity<ImageProduct>()
                .Property(e => e.HighResImageUrls)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                );

            modelBuilder.Entity<PhysicalProduct>()
                .Property(e => e.ShippingCountries)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                );

            modelBuilder.Entity<ServiceProduct>()
                .Property(e => e.AvailableSlots)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<DateTime>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<DateTime>()
                );

            modelBuilder.Entity<EventTicketProduct>()
                .Property(e => e.Includes)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions)null) ?? new List<string>()
                );
        }

        private void ConfigureIndexes(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopProduct>()
                .HasIndex(p => p.CreatorProfileId)
                .HasDatabaseName("IX_ShopProducts_CreatorProfileId");

            modelBuilder.Entity<ShopProduct>()
                .HasIndex(p => p.Status)
                .HasDatabaseName("IX_ShopProducts_Status");

            modelBuilder.Entity<ShopProduct>()
                .HasIndex(p => p.CreatedAt)
                .HasDatabaseName("IX_ShopProducts_CreatedAt");

            modelBuilder.Entity<PhysicalProduct>()
                .HasIndex(p => p.SKU)
                .IsUnique()
                .HasDatabaseName("IX_PhysicalProducts_SKU");

            modelBuilder.Entity<ShopOrder>()
                .HasIndex(o => o.OrderNumber)
                .IsUnique()
                .HasDatabaseName("IX_ShopOrders_OrderNumber");

            modelBuilder.Entity<ShopOrder>()
                .HasIndex(o => o.CustomerUserId)
                .HasDatabaseName("IX_ShopOrders_CustomerUserId");

            modelBuilder.Entity<ShopOrder>()
                .HasIndex(o => o.CreatorProfileId)
                .HasDatabaseName("IX_ShopOrders_CreatorProfileId");

            modelBuilder.Entity<ShopOrderItem>()
                .HasIndex(oi => oi.OrderId)
                .HasDatabaseName("IX_ShopOrderItems_OrderId");

            modelBuilder.Entity<ShopOrderItem>()
                .HasIndex(oi => oi.ProductId)
                .HasDatabaseName("IX_ShopOrderItems_ProductId");

            modelBuilder.Entity<ProductReview>()
                .HasIndex(r => r.ProductId)
                .HasDatabaseName("IX_ProductReviews_ProductId");

            modelBuilder.Entity<ProductReview>()
                .HasIndex(r => r.UserId)
                .HasDatabaseName("IX_ProductReviews_UserId");

            modelBuilder.Entity<ShopCategory>()
                .HasIndex(c => c.CreatorProfileId)
                .HasDatabaseName("IX_ShopCategories_CreatorProfileId");
        }

        private void ConfigureForeignKeys(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShopOrderItem>()
                .HasOne<ShopOrder>()
                .WithMany()
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}