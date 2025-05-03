using StorageService.Domain.entities;

namespace StorageService.Api.contracts.products;

public record class ProductDataResponse(
    string Id,
    string Name,
    string? Description,
    Dictionary<string, string> Props
)
{
    public static ProductDataResponse FromProduct(Product product) => new(
        product.Id.ToString(),
        product.Name,
        product.Description,
        product.Props.ToDictionary(p => p.Key, p => p.Value)
    );
}