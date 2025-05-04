using StorageService.Domain.common;
using StorageService.Domain.errs;
using StorageService.Domain.rules;

namespace StorageService.Domain.entities.storage;

public class Storage : Entity<StorageId>
{
    private Storage() { }
    public string Name { get; }
    private readonly List<ProductCountChangedRecord> _productCountChangedHistory;
    private readonly List<ProductRecord> _products;
    public CompanyAdminId AdminId { get; }
    public StorageProductAccessLevel ProductAccessLevel { get; private set; }
    public DateTime CreatedAt { get; }

    private Storage(
        StorageId id, CompanyAdminId adminId, string name,
        StorageProductAccessLevel productAccessLevel, DateTime createdAt
    ) {
        Id = id;
        AdminId = adminId;
        Name = name;
        _productCountChangedHistory = [];
        _products = [];
        ProductAccessLevel = productAccessLevel;
        CreatedAt = createdAt;
    }

    public static ErrOr<Storage> CreateNew(string name, CompanyAdminId adminId) {
        if (StorageRules.CheckStorageNameForErrs(name).IsErr(out var err)) {
            return err;
        }

        return new Storage(
            StorageId.CreateNew(), adminId, name, StorageProductAccessLevel.OnlyOwner, DateTime.UtcNow
        );
    }


    public uint AddProduct(ProductId productId, uint count) {
        ProductRecord? record = _products.FirstOrDefault(p => p.Id == productId);
        if (record is null) {
            _products.Add(ProductRecord.Create(productId, Id, count));
            _productCountChangedHistory.Add(ProductCountChangedRecord.CreateNew(
                Id, productId, ProductCountChangedType.Added, count
            ));
            return count;
        }

        record.AddCount(count);
        _productCountChangedHistory.Add(ProductCountChangedRecord.CreateNew(
            Id, productId, ProductCountChangedType.Added, count
        ));
        return record.Count;
    }

    public ErrOr<uint> RemoveProduct(ProductId productId, uint count) {
        ProductRecord? record = _products.FirstOrDefault(p => p.Id == productId);
        var current = record?.Count ?? 0;
        if (current < count) {
            return ErrFactory.InvalidData($"Not enough product in storage to remove. Current count: current");
        }

        record.RemoveCount(count);
        _productCountChangedHistory.Add(ProductCountChangedRecord.CreateNew(
            Id, productId, ProductCountChangedType.Removed, count
        ));
        return record.Count;
    }

    public void SetProductsAccessLevel(StorageProductAccessLevel accessLevel) {
        ProductAccessLevel = accessLevel;
    }
}