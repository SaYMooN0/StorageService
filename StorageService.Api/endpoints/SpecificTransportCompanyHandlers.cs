using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageService.Api.contracts.transport_companies;
using StorageService.Api.extensions;
using StorageService.Domain.common;
using StorageService.Domain.entities;
using StorageService.Domain.entities.storage;
using StorageService.Domain.entities.transport_company;
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

        endpoints.MapGet("/transport", TransportProducts)
            .WithAdminAuthRequired()
            .WithRequestValidation<ProductsTransportationRequest>()
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
        if (company is null) {
            return CustomResults.ErrorResponse(ErrFactory.NotFound("Transport company not found"));
        }

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

    private static async Task<IResult> TransportProducts(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "transportCompanyId")]
        string _
    ) {
        var transportCompanyId = httpContext.GetTransportCompanyIdFromRoute();
        var request = httpContext.GetValidatedRequest<ProductsTransportationRequest>();
        var adminId = httpContext.GetAuthenticatedAdminId();

        var sourceId = new StorageId(Guid.Parse(request.SourceStorageId));
        var destinationId = new StorageId(Guid.Parse(request.DestinationStorageId));

        var source = await dbContext.Storages
            .Include(s => EF.Property<List<ProductRecord>>(s, "_products"))
            .Include(s => EF.Property<List<ProductCountChangedRecord>>(s, "_productCountChangedHistory"))
            .FirstOrDefaultAsync(s => s.Id == sourceId);

        var destination = await dbContext.Storages
            .Include(s => EF.Property<List<ProductRecord>>(s, "_products"))
            .Include(s => EF.Property<List<ProductCountChangedRecord>>(s, "_productCountChangedHistory"))
            .FirstOrDefaultAsync(s => s.Id == destinationId);

        if (source is null || destination is null) {
            return CustomResults.ErrorResponse(ErrFactory.NotFound("One of the storages not found"));
        }

        if (source.AdminId != adminId) {
            return CustomResults.ErrorResponse(ErrFactory.Forbidden(
                "You do not have permission to transport products from the source storage"
            ));
        }

        if (destination.AdminId != adminId) {
            return CustomResults.ErrorResponse(ErrFactory.Forbidden(
                "You do not have permission to transport products to the destination storage"
            ));
        }

        Dictionary<ProductId, uint> parsedProducts = new();

        foreach (var (idStr, count) in request.ProductsWithCount) {
            var productId = new ProductId(Guid.Parse(idStr));
            var res = source.RemoveProduct(productId, (uint)count);

            if (res.IsErr(out var err)) {
                return CustomResults.ErrorResponse(err);
            }

            destination.AddProduct(productId, (uint)count);
            parsedProducts[productId] = (uint)count;
        }

        var record = TransportationRecord.CreateNew(transportCompanyId, sourceId, destinationId, parsedProducts);
        await dbContext.TransportationRecords.AddAsync(record);
        await dbContext.SaveChangesAsync();

        return Results.Ok(new { record.Id });
    }
}