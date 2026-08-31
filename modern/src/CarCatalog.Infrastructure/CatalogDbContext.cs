using CarCatalog.Domain;
using Microsoft.EntityFrameworkCore;

namespace CarCatalog.Infrastructure;

public class CatalogDbContext : DbContext
{
    public const string CatalogItemSequenceName = "catalog_hilo";

    private const int HiLoBlockSize = 10;

    /// <summary>
    /// The seed catalog occupies ids 1-12, so generated ids start after it.
    /// </summary>
    private const int FirstGeneratedCatalogItemId = 13;

    public CatalogDbContext(DbContextOptions<CatalogDbContext> options)
        : base(options)
    {
    }

    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();

    public DbSet<CatalogBrand> CatalogBrands => Set<CatalogBrand>();

    public DbSet<CatalogType> CatalogTypes => Set<CatalogType>();

    public DbSet<CatalogItemsStock> CatalogItemsStocks => Set<CatalogItemsStock>();

    public DbSet<DiscountItem> DiscountItems => Set<DiscountItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<CatalogType>(type =>
        {
            type.ToTable("CatalogType");
            type.HasKey(t => t.Id);
            type.Property(t => t.Id).ValueGeneratedNever();
            type.Property(t => t.Type).IsRequired().HasMaxLength(100);
        });

        builder.Entity<CatalogBrand>(brand =>
        {
            brand.ToTable("CatalogBrand");
            brand.HasKey(b => b.Id);
            brand.Property(b => b.Id).ValueGeneratedNever();
            brand.Property(b => b.Brand).IsRequired().HasMaxLength(100);
        });

        builder.Entity<CatalogItem>(item =>
        {
            item.ToTable("Catalog");
            item.HasKey(i => i.Id);

            // Keys came from a SQL sequence read in blocks of ten (the legacy CatalogItemHiLoGenerator);
            // EF Core's Hi-Lo generator does the same against the same sequence. Providers without
            // sequences fall back to their own generator.
            if (Database.IsSqlServer())
            {
                builder.HasSequence<int>(CatalogItemSequenceName)
                    .StartsAt(FirstGeneratedCatalogItemId)
                    .IncrementsBy(HiLoBlockSize);

                item.Property(i => i.Id).UseHiLo(CatalogItemSequenceName);
            }
            else
            {
                item.Property(i => i.Id).ValueGeneratedOnAdd();
            }

            item.Property(i => i.Name).IsRequired().HasMaxLength(50);
            item.Property(i => i.Price).HasColumnType("decimal(18,2)").IsRequired();
            item.Property(i => i.PictureFileName).IsRequired();
            item.Ignore(i => i.PictureUri);

            item.HasOne(i => i.CatalogBrand)
                .WithMany()
                .HasForeignKey(i => i.CatalogBrandId)
                .IsRequired();

            item.HasOne(i => i.CatalogType)
                .WithMany()
                .HasForeignKey(i => i.CatalogTypeId)
                .IsRequired();
        });

        builder.Entity<CatalogItemsStock>(stock =>
        {
            stock.ToTable("CatalogItemsStock");
            stock.HasKey(s => s.StockId);
            stock.Property(s => s.StockId).ValueGeneratedNever();
            stock.Property(s => s.Date).HasColumnType("date");
        });

        builder.Entity<DiscountItem>(discount =>
        {
            discount.ToTable("DiscountItem");
            discount.HasKey(d => d.Id);
            discount.Property(d => d.Start).HasColumnType("date");
            discount.Property(d => d.End).HasColumnType("date");
        });

        base.OnModelCreating(builder);
    }
}
