using StorageService.Domain.entities.storage;

namespace StorageService.Api.contracts.products;

public record class ProductInStoragesResponse(
    ProductWithCountResponse[] Storages
)
{
    public static ProductInStoragesResponse FromProductRecords(IEnumerable<ProductRecord> records) => new(
        records.Select(ProductWithCountResponse.FromProductRecord).ToArray()
    );
}