using StorageService.Domain.common;
using StorageService.Domain.errs;

namespace StorageService.Api;

public static class ErrPresets
{
    public static Err StorageNotFound(StorageId id) => ErrFactory.NotFound(
        "Storage not found", details: $"Cannot find storage with id {id}"
    ); 
    public static Err TransportCompanyNotFound(TransportCompanyId id) => ErrFactory.NotFound(
        "Transport company not found", details: $"Cannot find transport company with id {id}"
    );
    public static Err ProductNotFound(ProductId id) => ErrFactory.NotFound(
        "Product not found", details: $"Cannot find product with id {id}"
    );
}