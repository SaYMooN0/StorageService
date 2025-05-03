using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageService.Api.contracts.transport_companies;
using StorageService.Api.extensions;
using StorageService.Domain.entities;
using StorageService.Domain.errs;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints;

internal static class SpecificTransportCompanyHandlers
{
    internal static IEndpointRouteBuilder MapSpecificTransportCompanyHandlers(this RouteGroupBuilder endpoints) {
        endpoints.MapGet("/", GetTransportCompanyInfo);

        endpoints.MapPatch("/rename", RenameTransportCompany)
            .WithAdminAuthRequired()
            .WithRequestValidation<TransportCompanyNameSetRequest>()
            .WithAccessToAdminTransportCompanyRequired();

        endpoints.MapDelete("/delete", DeleteTransportCompany)
            .WithAdminAuthRequired()
            .WithAccessToAdminTransportCompanyRequired();

        return endpoints;
    }


    private static async Task<IResult> GetTransportCompanyInfo(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "transportCompanyId")]
        string _
    ) {
        var transportCompanyId = httpContext.GetTransportCompanyIdFromRoute();

        var company = await dbContext.TransportCompanies.FindAsync(transportCompanyId);
        if (company is null)
            return CustomResults.ErrorResponse(ErrFactory.NotFound("Transport company not found"));

        return Results.Json(new {
            Id = company.Id.Value,
            Name = company.Name,
            CreatedAt = company.CreatedAt
        });
    }

    private static async Task<IResult> RenameTransportCompany(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "transportCompanyId")]
        string _
    ) {
        var adminId = httpContext.GetAuthenticatedAdminId();
        var transportCompanyId = httpContext.GetTransportCompanyIdFromRoute();
        var request = httpContext.GetValidatedRequest<TransportCompanyNameSetRequest>();

        var admin = await dbContext.Admins
            .Include(p => EF.Property<List<TransportCompany>>(p, "_transportCompanies"))
            .FirstOrDefaultAsync(p => p.Id == adminId);

        if (admin is null) {
            return CustomResults.ErrorResponse(ErrPresets.AdminNotFound(adminId));
        }

        var renameRes = admin.RenameTransportCompany(transportCompanyId, request.Name);
        if (renameRes.IsErr(out var err)) {
            return CustomResults.ErrorResponse(err);
        }

        dbContext.Update(admin);
        await dbContext.SaveChangesAsync();
        return Results.Json(new { NewName = renameRes.AsSuccess() });
    }

    private static async Task<IResult> DeleteTransportCompany(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "transportCompanyId")]
        string _
    ) {
        var transportCompanyId = httpContext.GetTransportCompanyIdFromRoute();

        var company = await dbContext.TransportCompanies.FindAsync(transportCompanyId);
        if (company is null) {
            return CustomResults.ErrorResponse(ErrFactory.NotFound("Transport company not found"));
        }

        dbContext.TransportCompanies.Remove(company);
        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }
}