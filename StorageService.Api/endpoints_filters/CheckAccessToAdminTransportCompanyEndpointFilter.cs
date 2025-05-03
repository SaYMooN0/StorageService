using StorageService.Api.extensions;
using StorageService.Domain.common;
using StorageService.Domain.errs;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints_filters;

public class CheckAccessToAdminTransportCompanyEndpointFilter : IEndpointFilter
{
    private readonly AppDbContext _db;

    public CheckAccessToAdminTransportCompanyEndpointFilter(AppDbContext db) {
        _db = db;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next) {
        var companyIdStr = context.HttpContext.Request.RouteValues["transportCompanyId"]?.ToString() ?? "";
        if (!Guid.TryParse(companyIdStr, out var guid)) {
            return CustomResults.ErrorResponse(ErrFactory.InvalidData(
                "Invalid transport company id",
                $"Couldn't parse transport company id from route. Given value: {companyIdStr}"
            ));
        }

        TransportCompanyId companyId = new(guid);

        var company = await _db.TransportCompanies.FindAsync(companyId);
        if (company is null) {
            return CustomResults.ErrorResponse(ErrPresets.TransportCompanyNotFound(companyId));
        }

        var adminId = context.HttpContext.GetAuthenticatedAdminId();
        if (company.AdminId != adminId) {
            return CustomResults.ErrorResponse(ErrFactory.Forbidden(
                "You don't have access to administrate this transport company"
            ));
        }

        return await next(context);
    }
}