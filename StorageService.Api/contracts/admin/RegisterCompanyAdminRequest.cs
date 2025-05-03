using StorageService.Domain.rules;

namespace StorageService.Api.contracts;

internal class RegisterCompanyAdminRequest : IRequestWithValidationNeeded
{
    public string Name { get; init; }
    public string Password { get; init; }

    public RequestValidationResult Validate() {
       
        if (CompanyAdminRules.CheckAdminNameForErr(Name).IsErr(out var err)) {
            return err;
        }
        if (CompanyAdminRules.CheckPasswordForErr(Password).IsErr(out  err)) {
            return err;
        }

        return RequestValidationResult.Success;
    }

}