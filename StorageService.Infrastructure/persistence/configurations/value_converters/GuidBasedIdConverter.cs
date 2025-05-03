using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StorageService.Domain.common;

namespace StorageService.Infrastructure.persistence.configurations.value_converters;

public class GuidBasedIdConverter<TId> : ValueConverter<TId, Guid> where TId : GuidBasedId
{
    public GuidBasedIdConverter() : base(
        id => id.Value,
        value => (TId)Activator.CreateInstance(typeof(TId), value)
    ) { }
}