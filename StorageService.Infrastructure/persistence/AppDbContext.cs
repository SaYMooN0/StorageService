using System.Reflection;
using Microsoft.EntityFrameworkCore;
using StorageService.Domain.entities;

namespace StorageService.Infrastructure.persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Storage> Storages { get; init; } = null!;
    public DbSet<StorageAdmin> StorageAdmins { get; init; } = null!;
    public DbSet<TransportCompany> TransportCompanies { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
}