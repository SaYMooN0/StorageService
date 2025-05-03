using StorageService.Domain.rules;

namespace StorageService.Api.contracts.transport_companies;

public class TransportCompanyNameSetRequest : IRequestWithValidationNeeded
{
    public string Name { get; init; }

    public RequestValidationResult Validate() {
        if (TransportCompanyRules.CheckCompanyNameForErrs(Name).IsErr(out var err)) {
            return err;
        }

        return RequestValidationResult.Success;
    }
}