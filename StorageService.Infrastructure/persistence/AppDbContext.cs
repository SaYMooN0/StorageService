using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace StorageService.Infrastructure.persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        base.OnModelCreating(modelBuilder);
    }
}