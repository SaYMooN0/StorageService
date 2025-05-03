using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StorageService.Domain.common;
using StorageService.Domain.errs;

namespace StorageService.Api.extensions;

public static class HttpContextExtensions
{
    public static T GetValidatedRequest<T>(this HttpContext context) where T : class, IRequestWithValidationNeeded {
        if (!context.Items.TryGetValue("validatedRequest", out var validatedRequest)) {
            throw new InvalidDataException(
                "Trying to access validated request on the request that has not passed the validation"
            );
        }

        if (validatedRequest is T request) {
            return request;
        }

        throw new InvalidCastException("Request type mismatch");
    }

    public static CompanyAdminId GetAuthenticatedAdminId(this HttpContext context) {
        if (!context.Items.TryGetValue("adminId", out var userIdObj) || userIdObj is not CompanyAdminId id) {
            throw new UnauthorizedAccessException("User is not authenticated or AppUserId is missing");
        }

        return id;
    }

    public static ErrOr<CompanyAdminId> ParseAdminIdFromJwtToken(
        this HttpContext httpContext, JwtTokenConfig jwtConfig
    ) {
        if (!httpContext.Request.Cookies.TryGetValue("_token", out var token) || string.IsNullOrEmpty(token)) {
            return ErrFactory.AuthRequired(
                "Company admin is not authenticated",
                details: "Log in to your account"
            );
        }

        JwtSecurityTokenHandler handler = new();
        TokenValidationParameters tokenValidationParameters = new() {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = jwtConfig.Issuer,
            ValidAudience = jwtConfig.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
        };

        try {
            var principal = handler.ValidateToken(token, tokenValidationParameters, out _);
            var userIdClaim = principal.Claims.FirstOrDefault(c => c.Type == "adminId")?.Value;

            if (string.IsNullOrEmpty(userIdClaim)) {
                return ErrFactory.AuthRequired(
                    "Company admin is not authenticated",
                    details: "Log in to your account"
                );
            }

            return new CompanyAdminId(Guid.Parse(userIdClaim));
        }
        catch (Exception) {
            return ErrFactory.AuthRequired(
                "Company admin is not authenticated",
                details: "Log in to your account"
            );
        }
    }

    public static ProductId GetProductIdFromRoute(this HttpContext context) {
        var productIdString = context.Request.RouteValues["productId"]?.ToString() ?? "";
        if (!Guid.TryParse(productIdString, out var guid)) {
            throw new Exception($"Couldn't parse product id from route. Given value: {productIdString}");
        }

        return new ProductId(guid);
    }

    public static StorageId GetStorageIdFromRoute(this HttpContext context) {
        var productIdString = context.Request.RouteValues["storageId"]?.ToString() ?? "";
        if (!Guid.TryParse(productIdString, out var guid)) {
            throw new Exception($"Couldn't parse storage id from route. Given value: {productIdString}");
        }

        return new StorageId(guid);
    }
    public static TransportCompanyId GetTransportCompanyIdFromRoute(this HttpContext context) {
        var productIdString = context.Request.RouteValues["transportCompanyId"]?.ToString() ?? "";
        if (!Guid.TryParse(productIdString, out var guid)) {
            throw new Exception($"Couldn't parse transport company id from route. Given value: {productIdString}");
        }

        return new TransportCompanyId(guid);
    }
}