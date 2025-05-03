using StorageService.Api.extensions;
using StorageService.Domain.common;
using StorageService.Domain.errs;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints_filters;

public class CheckAccessToAdminStorageEndpointFilter : IEndpointFilter
{
    private readonly AppDbContext _db;
    
    public CheckAccessToAdminStorageEndpointFilter(AppDbContext db) {
        _db = db;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        var storageIdStr = context.HttpContext.Request.RouteValues["storageId"]?.ToString() ?? "";
        if (!Guid.TryParse(storageIdStr, out var guid)) {
            return CustomResults.ErrorResponse(ErrFactory.InvalidData(
                "Invalid storage id",
                $"Couldn't parse storage id from route. Given value: {storageIdStr}"
            ));
        }

        StorageId storageId = new(guid);

        var storage = await _db.Storages.FindAsync(storageId);
        if (storage is null) {
            return CustomResults.ErrorResponse(ErrPresets.StorageNotFound(storageId));
        }

        var adminId = context.HttpContext.GetAuthenticatedAdminId();
        if (storage.AdminId != adminId) {
            return CustomResults.ErrorResponse(ErrFactory.Forbidden(
                "You don't have access to administrate this storage"
            ));
        }

        return await next(context);
    }
}