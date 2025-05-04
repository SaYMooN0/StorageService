using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.entities;
using StorageService.Infrastructure.persistence.configurations.value_converters;

namespace StorageService.Infrastructure.persistence.configurations.entities_configurations.storage;

internal class ProductConfigurations : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();

        builder.Property(x => x.Name);
        builder.Property(x => x.Description);
        
        builder
            .Property(x => x.Props)
            .HasConversion<ImmutableDictionaryConverter>()
            .HasColumnType("jsonb");
    }
}