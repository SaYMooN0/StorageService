using StorageService.Domain.entities.storage;

namespace StorageService.Api.contracts.storages;

public class StorageProductCountChangeResponse(
    string ProductId,
    string Type, // Added / Removed
    uint Count,
    DateTime ChangedAt
)
{
    public static StorageProductCountChangeResponse FromEntity(ProductCountChangedRecord record) => new(
        record.ProductId.ToString(),
        record.Type.ToString(),
        record.Count,
        record.DateTime
    );
}