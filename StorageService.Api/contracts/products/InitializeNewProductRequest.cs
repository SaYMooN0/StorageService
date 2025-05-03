using StorageService.Domain.rules;

namespace StorageService.Api.contracts.products;

public class InitializeNewProductRequest : IRequestWithValidationNeeded
{
    public string Name { get; init; }
    public string? Description { get; init; }
    public Dictionary<string, string> Props { get; init; }

    public RequestValidationResult Validate() {
        if (ProductRules.CheckNameForErrs(Name).IsErr(out var err)) {
            return err;
        }

        if (ProductRules.CheckDescriptionForErrs(Description).IsErr(out err)) {
            return err;
        }

        if (ProductRules.CheckPropsForErrs(Props).IsErr(out err)) {
            return err;
        }
        return RequestValidationResult.Success;
    }
}