using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.entities.storage;

namespace StorageService.Infrastructure.persistence.configurations.entities_configurations;

internal class ProductRecordConfigurations: IEntityTypeConfiguration<ProductRecord>
{
    public void Configure(EntityTypeBuilder<ProductRecord> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();

        builder.Property(x => x.Count);
        
        builder
            .Property(x => x.StorageId)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();
        
        builder
            .Property(x => x.ProductId)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();
    }
}