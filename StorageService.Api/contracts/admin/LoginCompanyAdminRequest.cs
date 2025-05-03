using StorageService.Domain.errs;

namespace StorageService.Api.contracts.admin;

public class LoginCompanyAdminRequest : IRequestWithValidationNeeded
{
    public string Name { get; init; }
    public string Password { get; init; }

    public RequestValidationResult Validate() {
        if (string.IsNullOrWhiteSpace(Name)) {
            return ErrFactory.NoValue("Name is required");
        }

        if (string.IsNullOrWhiteSpace(Password)) {
            return ErrFactory.InvalidData(message: "Password is required");
        }

        return RequestValidationResult.Success; 
    }

}