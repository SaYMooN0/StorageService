using StorageService.Domain.errs;

namespace StorageService.Api.contracts.transport_companies;

public class ProductsTransportationRequest : IRequestWithValidationNeeded
{
    public string SourceStorageId { get; init; }
    public string DestinationStorageId { get; init; }
    public Dictionary<string, int> ProductsWithCount { get; init; }

    public RequestValidationResult Validate() {
        if (!Guid.TryParse(SourceStorageId, out _)) {
            return ErrFactory.InvalidData(
                "Invalid source storage id", $"{SourceStorageId} is not a valid storage id"
            );
        }

        if (!Guid.TryParse(DestinationStorageId, out _)) {
            return ErrFactory.InvalidData(
                "Invalid destination storage id", $"{SourceStorageId} is not a valid storage id"
            );
        }

        if (ProductsWithCount.Count == 0) {
            return ErrFactory.NoValue("You must specify at least one product to transport");
        }

        if (SourceStorageId == DestinationStorageId) {
            return ErrFactory.NoValue("Source and destination storage cannot be the same");
        }

        foreach (var (productIdStr, count) in ProductsWithCount) {
            if (!Guid.TryParse(productIdStr, out _)) {
                return ErrFactory.InvalidData(
                    "Invalid product id",
                    $"Product id {productIdStr} is not a valid product id"
                );
            }

            if (count <= 0) {
                return ErrFactory.InvalidData(
                    "Invalid product count",
                    $"Product {productIdStr} has invalid count: {count}. Count must be greater than 0"
                );
            }
        }

        return RequestValidationResult.Success;
    }
}