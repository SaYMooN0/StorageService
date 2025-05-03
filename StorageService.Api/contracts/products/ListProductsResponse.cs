using StorageService.Domain.entities;

namespace StorageService.Api.contracts.products;

public record class ListProductsResponse(
    ProductDataResponse[] Products
)
{
    public static ListProductsResponse FromProducts(List<Product> products) => new(
        products.Select(ProductDataResponse.FromProduct).ToArray()
    );
}