using StorageService.Domain.common;

namespace StorageService.Domain.entities.storage;

public class ProductCountChangedRecord : Entity<ProductCountChangedRecordId>
{
    private ProductCountChangedRecord() { }
    public ProductCountChangedType Type { get; }
    public DateTime DateTime { get; }
    public ProductId ProductId { get; }
    public StorageId StorageId { get; }
    public uint Count { get; }

    private ProductCountChangedRecord(
        ProductCountChangedRecordId id,
        StorageId storageId,
        ProductId productId,
        DateTime dateTime,
        ProductCountChangedType type,
        uint count
    ) {
        DateTime = dateTime;
        ProductId = productId;
        Type = type;
        Count = count;
        StorageId = storageId;
    }

    public static ProductCountChangedRecord CreateNew(
        StorageId storageId, ProductId productId, ProductCountChangedType type, uint count
    ) => new(
        ProductCountChangedRecordId.CreateNew(), storageId, productId, DateTime.Now, type, count
    );
}

public enum ProductCountChangedType
{
    Added,
    Removed,
}