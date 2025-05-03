using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StorageService.Domain.common;
using StorageService.Infrastructure.persistence.configurations.value_converters;

namespace StorageService.Infrastructure.persistence.configurations;

public static class PropertyBuilderExtensions
{
    public static PropertyBuilder<TId> HasGuidBasedIdConversion<TId>(
        this PropertyBuilder<TId> builder
    ) where TId : GuidBasedId => builder.HasConversion<GuidBasedIdConverter<TId>>();
}