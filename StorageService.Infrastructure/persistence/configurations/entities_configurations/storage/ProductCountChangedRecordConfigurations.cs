using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.entities.storage;

namespace StorageService.Infrastructure.persistence.configurations.entities_configurations.storage;

internal class ProductCountChangedRecordConfigurations: IEntityTypeConfiguration<ProductCountChangedRecord>
{
    public void Configure(EntityTypeBuilder<ProductCountChangedRecord> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();

        builder.Property(x=>x.DateTime);
        builder.Property(x=>x.Count);
        
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