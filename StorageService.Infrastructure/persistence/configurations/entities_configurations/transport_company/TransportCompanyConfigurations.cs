using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.entities.transport_company;

namespace StorageService.Infrastructure.persistence.configurations.entities_configurations.transport_company;

internal class TransportCompanyConfigurations : IEntityTypeConfiguration<TransportCompany>
{
    public void Configure(EntityTypeBuilder<TransportCompany> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();

        builder.Property(x => x.Name);
        builder.Property(x => x.CreatedAt);

        builder
            .Property(x => x.AdminId)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();

        builder
            .HasMany<TransportationRecord>("_transportations")
            .WithOne()
            .HasForeignKey(x => x.TransportCompanyId);
    }
}