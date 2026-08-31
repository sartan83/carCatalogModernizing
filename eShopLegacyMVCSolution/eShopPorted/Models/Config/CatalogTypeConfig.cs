using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace eShopPorted.Models.Config
{
    public class CatalogTypeConfig : IEntityTypeConfiguration<CatalogType>
    {
        public void Configure(EntityTypeBuilder<CatalogType> builder)
        {
            builder.ToTable("CatalogType");

            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Id)
               .IsRequired();

            builder.Property(cb => cb.Type)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasData(
                new CatalogType { Id = 1, Type = "Sports Car" },
                new CatalogType { Id = 2, Type = "GT" },
                new CatalogType { Id = 3, Type = "SUV" },
                new CatalogType { Id = 4, Type = "Spare Part" }
            );
        }
    }
}
