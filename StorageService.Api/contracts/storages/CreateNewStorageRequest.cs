using StorageService.Domain.rules;

namespace StorageService.Api.contracts.storages;

public class CreateNewStorageRequest : IRequestWithValidationNeeded
{
    public string Name { get; init; }

    public RequestValidationResult Validate() {
        if (StorageRules.CheckStorageNameForErrs(Name).IsErr(out var err)) {
            return err;
        }

        return RequestValidationResult.Success;
    }
}