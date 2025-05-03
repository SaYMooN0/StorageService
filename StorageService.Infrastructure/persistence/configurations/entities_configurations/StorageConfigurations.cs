using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.entities;
using StorageService.Domain.entities.storage;

namespace StorageService.Infrastructure.persistence.configurations.entities_configurations;

internal  class StorageConfigurations: IEntityTypeConfiguration<Storage>
{
    public void Configure(EntityTypeBuilder<Storage> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();
    }
}