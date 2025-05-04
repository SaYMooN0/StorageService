using StorageService.Api.extensions;
using StorageService.Domain.common;
using StorageService.Domain.entities.storage;
using StorageService.Domain.errs;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints_filters;

public class CheckStorageProductAccessEndpointFilter : IEndpointFilter
{
    private readonly AppDbContext _db;
    private readonly JwtTokenConfig _jwtConfig;

    public CheckStorageProductAccessEndpointFilter(AppDbContext db, JwtTokenConfig jwtConfig) {
        _db = db;
        _jwtConfig = jwtConfig;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        var httpContext = context.HttpContext;
        var storageIdStr = httpContext.Request.RouteValues["storageId"]?.ToString() ?? "";

        if (!Guid.TryParse(storageIdStr, out var guid)) {
            return CustomResults.ErrorResponse(ErrFactory.InvalidData(
                "Invalid storage ID",
                $"Could not parse storage ID from route. Given: {storageIdStr}"
            ));
        }

        StorageId storageId = new(guid);

        var storage = await _db.Storages.FindAsync(storageId);
        if (storage is null) {
            return CustomResults.ErrorResponse(ErrPresets.StorageNotFound(storageId));
        }

        if (storage.ProductAccessLevel == StorageProductAccessLevel.Public) {
            return await next(context);
        }

        var adminIdOrErr = httpContext.ParseAdminIdFromJwtToken(_jwtConfig);
        if (adminIdOrErr.IsErr(out var err)) {
            return CustomResults.ErrorResponse(ErrFactory.AuthRequired(
                "You must be logged in to access products in this storage"
            ));
        }

        var adminId = adminIdOrErr.AsSuccess();

        if (storage.ProductAccessLevel == StorageProductAccessLevel.Authenticated) {
            return await next(context);
        }

        if (storage.ProductAccessLevel == StorageProductAccessLevel.OnlyOwner && storage.AdminId == adminId) {
            return await next(context);
        }

        return CustomResults.ErrorResponse(
            ErrFactory.Forbidden("You don't have access to view products in this storage"));
    }
}