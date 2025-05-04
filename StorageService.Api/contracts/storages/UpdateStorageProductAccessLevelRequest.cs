using StorageService.Domain.entities.storage;

namespace StorageService.Api.contracts.storages;

public class UpdateStorageProductAccessLevelRequest : IRequestWithValidationNeeded
{
    public StorageProductAccessLevel AccessLevel { get; init; }

    public RequestValidationResult Validate() => RequestValidationResult.Success;
}