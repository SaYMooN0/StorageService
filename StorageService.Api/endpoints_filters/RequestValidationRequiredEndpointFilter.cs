using StorageService.Domain.errs;

namespace StorageService.Api.endpoints_filters;

internal class RequestValidationRequiredEndpointFilter<T> : IEndpointFilter
    where T : class, IRequestWithValidationNeeded
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        var httpContext = context.HttpContext;

        //for not application/json type requests
        bool isContentJson =
            httpContext.Request.ContentType?.StartsWith("application/json", StringComparison.OrdinalIgnoreCase) ??
            false;
        if (!isContentJson) {
            return CustomResults.ErrorResponse(ErrFactory.InvalidData(
                "Invalid Content-Type. Expected application/json"
            ));
        }

        //for empty body requests
        if (httpContext.Request.ContentLength == 0) {
            return CustomResults.ErrorResponse(ErrFactory.NoValue(
                "Request body cannot be empty when Content-Type is application/json"
            ));
        }   

        T request = null;

        try {
            request = await httpContext.Request.ReadFromJsonAsync<T>();
        }
        catch (System.Text.Json.JsonException exception) {
            return CustomResults.ErrorResponse(new Err("Unable to read from json"));
        }

        if (request is not T validatableRequest) {
            return CustomResults.ErrorResponse(ErrFactory.IncorrectFormat(
                "Invalid request body format"
            ));
        }

        var validationResult = validatableRequest.Validate();
        if (validationResult.IsErr(out var errsArray)) {
            return CustomResults.ErrorResponse(errsArray);
        }

        httpContext.Items["validatedRequest"] = validatableRequest;

        return await next(context);
    }
}