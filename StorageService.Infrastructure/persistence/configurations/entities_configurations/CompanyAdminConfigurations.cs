using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.entities;
using StorageService.Domain.entities.storage;

namespace StorageService.Infrastructure.persistence.configurations.entities_configurations;

internal class CompanyAdminConfigurations : IEntityTypeConfiguration<CompanyAdmin>
{
    public void Configure(EntityTypeBuilder<CompanyAdmin> builder) {
        builder
            .HasKey(x => x.Id);
        builder
            .Property(x => x.Id)
            .ValueGeneratedNever()
            .HasGuidBasedIdConversion();

        builder.Property(x => x.Name);
        builder.Property(x => x.PasswordHash);
        builder.Property(x => x.RegistrationDate);

        builder.Ignore(x => x.Storages);
        builder
            .HasMany<Storage>("_storages")
            .WithOne()
            .HasForeignKey(x => x.AdminId); 
        
        builder.Ignore(x => x.TransportCompanies);
        builder
            .HasMany<TransportCompany>("_transportCompanies")
            .WithOne()
            .HasForeignKey(x => x.AdminId);
    }
}