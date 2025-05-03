using StorageService.Domain.common;

namespace StorageService.Domain.entities.storage;

public class ProductCountChangedRecord : Entity<ProductCountChangedRecordId>
{
    private ProductCountChangedRecord() { }
    public ProductCountChangedType Type { get; }
    public DateTime DateTime { get; }
    public ProductId ProductId { get; }
    public uint Count { get; }

    private ProductCountChangedRecord(
        ProductCountChangedRecordId id,
        DateTime dateTime,
        ProductId productId,
        ProductCountChangedType type,
        uint count
    ) {
        DateTime = dateTime;
        ProductId = productId;
        Type = type;
        Count = count;
    }

    public static ProductCountChangedRecord CreateNew(ProductId productId, ProductCountChangedType type, uint count) =>
        new(ProductCountChangedRecordId.CreateNew(), DateTime.Now, productId, type, count);
}

public enum ProductCountChangedType
{
    Added,
    Removed,
}