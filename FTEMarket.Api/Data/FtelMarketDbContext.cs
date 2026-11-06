using FTEMarket.Api.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FTEMarket.Api.Data;

public class FtelMarketDbContext(DbContextOptions<FtelMarketDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FtelMarketDbContext).Assembly);
}
