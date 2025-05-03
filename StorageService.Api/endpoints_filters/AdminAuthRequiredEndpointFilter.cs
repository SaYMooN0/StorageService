using StorageService.Api.extensions;
using StorageService.Domain.errs;

namespace StorageService.Api.endpoints_filters;

internal class AdminAuthRequiredEndpointFilter : IEndpointFilter
{
    private readonly JwtTokenConfig _jwtConfig;

    public AdminAuthRequiredEndpointFilter(JwtTokenConfig config) {
        _jwtConfig = config;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        var httpContext = context.HttpContext;
        var userIdOrErr = httpContext.ParseAdminIdFromJwtToken(_jwtConfig);

        if (userIdOrErr.IsErr(out var err)) {
            return CustomResults.ErrorResponse(ErrFactory.AuthRequired(
                "Access denied. Admin authentication required. Please log into your account"
            ));
        }

        httpContext.Items["adminId"] = userIdOrErr.AsSuccess();

        return await next(context);
    }
}