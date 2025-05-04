using StorageService.Domain.errs;

namespace StorageService.Api.contracts.storages;

public class ChangeProductCountRequest : IRequestWithValidationNeeded
{
    public string ProductId { get; init; }
    public uint Count { get; init; }

    public RequestValidationResult Validate() {
        if (!Guid.TryParse(ProductId, out _)) {
            return ErrFactory.IncorrectFormat(
                "Unable to parse product Id",
                $"Given value {ProductId} is not a valid product id"
            );
        }

        if (Count < 1) {
            return ErrFactory.InvalidData("Cannot process operation with less than 1 product");
        }


        return RequestValidationResult.Success;
    }
}