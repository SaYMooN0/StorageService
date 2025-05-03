using StorageService.Api.endpoints_filters;

namespace StorageService.Api.extensions;

public static class EndpointRouteBuilderExtensions
{
    public static RouteHandlerBuilder WithRequestValidation<T>(this RouteHandlerBuilder builder)
        where T : class, IRequestWithValidationNeeded {
        return builder
            .Accepts<T>("application/json")
            .AddEndpointFilter<RequestValidationRequiredEndpointFilter<T>>();
    }

    public static RouteHandlerBuilder WithAdminAuthRequired(this RouteHandlerBuilder builder) {
        return builder.AddEndpointFilter<AdminAuthRequiredEndpointFilter>();
    }
}