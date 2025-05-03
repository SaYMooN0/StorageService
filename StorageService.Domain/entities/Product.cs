using System.Collections.Immutable;
using StorageService.Domain.common;

namespace StorageService.Domain.entities;

public class Product : Entity<ProductId>
{
    public string Name { get; }
    public string?  Description { get; }
    public ImmutableDictionary<string, string> Props { get; }
}