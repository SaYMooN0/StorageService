using System.Collections.Immutable;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace StorageService.Infrastructure.persistence.configurations.value_converters;

public class ImmutableDictionaryDictionaryConverter : ValueConverter<ImmutableDictionary<string, string>, string>
{
    public ImmutableDictionaryDictionaryConverter() : base(
        v => ToJson(v),
        v => FromJson(v)
    ) { }

    private static string ToJson(ImmutableDictionary<string, string> v) =>
        JsonSerializer.Serialize(v);

    private static ImmutableDictionary<string, string> FromJson(string v) =>
        JsonSerializer.Deserialize<ImmutableDictionary<string, string>>(v);
}