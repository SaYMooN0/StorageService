using StorageService.Domain.common;
using StorageService.Domain.errs;

namespace StorageService.Domain.entities.storage;

public class ProductRecord : Entity<ProductRecordId>
{
    private ProductRecord() { }
    public Product Product { get; }
    public ProductId ProductId { get; }
    public uint Count { get; private set; }

    private ProductRecord(ProductRecordId id, ProductId productId, uint count) {
        Id = id;
        ProductId = productId;
        Count = count;
    }

    public static ProductRecord Create(ProductId productId, uint count) =>
        new(ProductRecordId.CreateNew(), productId, count);


    public void AddCount(uint count) => Count += count;

    public ErrOrNothing RemoveCount(uint count) {
        if (count > Count) {
            return ErrFactory.Conflict("Cannot remove more products than its count");
        }

        Count -= count;
        return ErrOrNothing.Nothing;
    }
}