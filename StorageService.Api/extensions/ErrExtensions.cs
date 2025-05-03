using StorageService.Domain.errs;

namespace StorageService.Api.extensions;

public static class ErrExtensions
{
    public static int ToHttpStatusCode(this Err e) => e.Code switch {
        // Validation
        >= 1100 and < 1200 => StatusCodes.Status400BadRequest,

        // Business logic
        Err.ErrCodes.Conflict => StatusCodes.Status409Conflict,
        Err.ErrCodes.NotFound => StatusCodes.Status404NotFound,

        // Auth
        Err.ErrCodes.AuthRequired => StatusCodes.Status401Unauthorized,
        Err.ErrCodes.NoAccess or Err.ErrCodes.Forbidden => StatusCodes.Status403Forbidden,

        // Default fallback
        _ => StatusCodes.Status500InternalServerError
    };
}