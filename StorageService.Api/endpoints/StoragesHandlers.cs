using Microsoft.EntityFrameworkCore;
using StorageService.Api.contracts.storages;
using StorageService.Api.extensions;
using StorageService.Domain.entities.storage;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints;

internal static class StoragesHandlers
{
    internal static IEndpointRouteBuilder MapStoragesHandlers(this RouteGroupBuilder endpoints) {
        endpoints.MapPost("/create-new", CreateNewStorage)
            .WithAdminAuthRequired()
            .WithRequestValidation<CreateNewStorageRequest>();

        endpoints.MapGet("/count", CountMyStorages)
            .WithAdminAuthRequired();

        return endpoints;
    }

    private static async Task<IResult> CreateNewStorage(
        HttpContext httpContext,
        AppDbContext dbContext
    ) {
        var adminId = httpContext.GetAuthenticatedAdminId();
        var request = httpContext.GetValidatedRequest<CreateNewStorageRequest>();
        var admin = await dbContext.Admins
            .WithStorages()
            .FirstOrDefaultAsync(p => p.Id == adminId);

        if (admin is null) {
            return CustomResults.ErrorResponse(ErrPresets.AdminNotFound(adminId));
        }
        
        var storageRes = admin.CreateStorage(request.Name);
        if (storageRes.IsErr(out var err)) {
            return CustomResults.ErrorResponse(err);
        }

        Storage storage = storageRes.AsSuccess();

        await dbContext.Storages.AddAsync(storage);
        await dbContext.SaveChangesAsync();

        return CustomResults.Created(new { Id = storage.Id });
    }

    private static async Task<IResult> CountMyStorages(
        HttpContext httpContext,
        AppDbContext dbContext
    ) {
        var adminId = httpContext.GetAuthenticatedAdminId();
        int count = await dbContext.Storages
            .Where(s => s.AdminId == adminId)
            .CountAsync();

        return Results.Json(new { Count = count });
    }
}