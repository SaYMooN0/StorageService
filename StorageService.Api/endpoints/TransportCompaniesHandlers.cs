using Microsoft.EntityFrameworkCore;
using StorageService.Api.contracts.transport_companies;
using StorageService.Api.extensions;
using StorageService.Domain.entities;
using StorageService.Domain.entities.transport_company;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints;

internal static class TransportCompaniesHandlers
{
    internal static IEndpointRouteBuilder MapTransportCompaniesHandlers(this RouteGroupBuilder endpoints) {
        endpoints.MapPost("/create-new", CreateNewTransportCompany)
            .WithAdminAuthRequired()
            .WithRequestValidation<TransportCompanyNameSetRequest>();

        endpoints.MapGet("/count", CountMyTransportCompanies)
            .WithAdminAuthRequired();

        return endpoints;
    }

    private static async Task<IResult> CreateNewTransportCompany(
        HttpContext httpContext,
        AppDbContext dbContext
    ) {
        var adminId = httpContext.GetAuthenticatedAdminId();
        var request = httpContext.GetValidatedRequest<TransportCompanyNameSetRequest>();

        var admin = await dbContext.Admins
            .Include(p => EF.Property<List<TransportCompany>>(p, "_transportCompanies"))
            .FirstOrDefaultAsync(p => p.Id == adminId);

        if (admin is null) {
            return CustomResults.ErrorResponse(ErrPresets.AdminNotFound(adminId));
        }

        var companyRes = admin.CreateTransportCompany(request.Name);
        if (companyRes.IsErr(out var err)) {
            return CustomResults.ErrorResponse(err);
        }

        TransportCompany company = companyRes.AsSuccess();

        await dbContext.TransportCompanies.AddAsync(company);
        await dbContext.SaveChangesAsync();

        return CustomResults.Created(new { Id = company.Id });
    }

    private static async Task<IResult> CountMyTransportCompanies(
        HttpContext httpContext,
        AppDbContext dbContext
    ) {
        var adminId = httpContext.GetAuthenticatedAdminId();
        int count = await dbContext.TransportCompanies
            .Where(c => c.AdminId == adminId)
            .CountAsync();

        return Results.Json(new { Count = count });
    }
}