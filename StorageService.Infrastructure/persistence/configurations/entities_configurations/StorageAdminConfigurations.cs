using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.entities;

namespace StorageService.Infrastructure.persistence.configurations.entities_configurations;

internal  class StorageAdminConfigurations: IEntityTypeConfiguration<StorageAdmin>
{
    public void Configure(EntityTypeBuilder<StorageAdmin> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();
    }
}