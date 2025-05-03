using StorageService.Domain.errs;

namespace StorageService.Api;

public interface IRequestWithValidationNeeded
{
    public RequestValidationResult Validate();
}

public class RequestValidationResult
{
    private ErrOrNothing _err;

    public RequestValidationResult(ErrOrNothing err) {
        _err = err;
    }

    public bool IsErr(out Err err) {
        if (_err.IsErr(out var errObj)) {
            err = errObj;
            return true;
        }

        err = null;
        return false;
    }

    public static implicit operator RequestValidationResult(Err err) => new(err);


    public static readonly RequestValidationResult Success = new(ErrOrNothing.Nothing);
}