using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StorageService.Domain.common;

namespace StorageService.Infrastructure.persistence.configurations.value_converters;

public class ProductsWithCountConverter: ValueConverter<ImmutableDictionary<ProductId, uint>, string>
{
    public ProductsWithCountConverter() : base(
        v => ToJson(v),
        v => FromJson(v)
    ) { }

    private static string ToJson(ImmutableDictionary<ProductId, uint> v) =>
        JsonSerializer.Serialize(v);

    private static ImmutableDictionary<ProductId, uint> FromJson(string v) =>
        JsonSerializer.Deserialize<ImmutableDictionary<ProductId, uint>>(v);
}