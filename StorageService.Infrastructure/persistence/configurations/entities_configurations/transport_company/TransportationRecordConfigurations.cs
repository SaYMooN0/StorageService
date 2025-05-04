using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.entities.transport_company;
using StorageService.Infrastructure.persistence.configurations.value_converters;

namespace StorageService.Infrastructure.persistence.configurations.entities_configurations.transport_company;

public class TransportationRecordConfigurations : IEntityTypeConfiguration<TransportationRecord>
{
    public void Configure(EntityTypeBuilder<TransportationRecord> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();

        builder
            .Property(x => x.SourceStorageId)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();
        builder
            .Property(x => x.DestinationStorageId)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();
        builder
            .Property(x => x.TransportedAt);

        builder
            .Property(x => x.ProductsWithCount)
            .HasConversion<ProductsWithCountConverter>();
    }
}