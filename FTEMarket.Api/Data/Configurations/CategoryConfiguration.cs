using FTEMarket.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FTEMarket.Api.Data.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.Property(c => c.Name).HasMaxLength(100).IsRequired();

        builder.HasData(
            new Category { Id = 1, Name = "PlayStation 5" },
            new Category { Id = 2, Name = "Nintendo Switch" },
            new Category { Id = 3, Name = "Xbox Series X" },
            new Category { Id = 4, Name = "PC" });
    }
}
