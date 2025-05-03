using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StorageService.Domain.entities;
using StorageService.Domain.entities.storage;

namespace StorageService.Infrastructure.persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<CompanyAdmin> Admins { get; init; } = null!;
    public DbSet<Storage> Storages { get; init; } = null!;
    public DbSet<TransportCompany> TransportCompanies { get; init; } = null!;
    public DbSet<Product> Products { get; init; } = null!;
    public DbSet<ProductRecord> ProductRecords { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}