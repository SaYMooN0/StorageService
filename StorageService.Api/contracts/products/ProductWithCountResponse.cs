using StorageService.Domain.entities.storage;

namespace StorageService.Api.contracts.products;

public class ProductWithCountResponse(string StorageId, uint Count)
{
    public static ProductWithCountResponse FromProductRecord(ProductRecord record) => new(
        record.StorageId.ToString(),
        record.Count
    );
}