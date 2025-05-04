using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageService.Api.contracts.storages;
using StorageService.Api.extensions;
using StorageService.Domain.common;
using StorageService.Domain.errs;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints;

internal static class SpecificStorageHandlers
{
    internal static IEndpointRouteBuilder MapSpecificStorageHandlers(this RouteGroupBuilder endpoints) {
        endpoints.MapGet("/products-count", GetProductsCount)
            .WithAccessToViewProductsInStorageRequired();
        endpoints.MapGet("/products-with-count", GetProductsWithCount)
            .WithAccessToViewProductsInStorageRequired();
        endpoints.MapGet("/products-full-with-count", GetProductsFullInfoWithCount)
            .WithAccessToViewProductsInStorageRequired();

        endpoints.MapGet("/history", GetStorageHistory)
            .WithAdminAuthRequired()
            .WithAccessToAdminStorageRequired();

        endpoints.MapPost("/add-product", AddProductToStorage)
            .WithAdminAuthRequired()
            .WithRequestValidation<ChangeProductCountRequest>()
            .WithAccessToAdminStorageRequired();

        endpoints.MapPost("/remove-product", RemoveProductFromStorage)
            .WithAdminAuthRequired()
            .WithRequestValidation<ChangeProductCountRequest>()
            .WithAccessToAdminStorageRequired();

        endpoints.MapPatch("/access-level", UpdateProductAccessLevel)
            .WithAdminAuthRequired()
            .WithAccessToAdminStorageRequired()
            .WithRequestValidation<UpdateStorageProductAccessLevelRequest>();

        endpoints.MapDelete("/delete", DeleteStorage)
            .WithAdminAuthRequired()
            .WithAccessToAdminStorageRequired();


        return endpoints;
    }

    private static async Task<IResult> GetProductsWithCount(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "storageId")] string _,
        [FromQuery] uint min = 1
    ) {
        var storageId = httpContext.GetStorageIdFromRoute();

        var records = await dbContext.ProductRecords
            .Where(r => r.StorageId == storageId && r.Count >= min)
            .ToListAsync();

        var result = records.Select(r => new {
            ProductId = r.ProductId.Value,
            r.Count
        });

        return Results.Json(result);
    }

    private static async Task<IResult> GetProductsFullInfoWithCount(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "storageId")] string _,
        [FromQuery] uint min = 1
    ) {
        var storageId = httpContext.GetStorageIdFromRoute();

        var records = await dbContext.ProductRecords
            .Include(r => r.Product)
            .Where(r => r.StorageId == storageId && r.Count >= min)
            .ToListAsync();

        var result = records.Select(r => new {
            ProductId = r.Product.Id.Value,
            r.Product.Name,
            r.Product.Description,
            Props = r.Product.Props.ToDictionary(p => p.Key, p => p.Value),
            r.Count
        });

        return Results.Json(result);
    }

    private static async Task<IResult> GetProductsCount(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "storageId")] string _,
        [FromQuery] uint min = 1
    ) {
        var storageId = httpContext.GetStorageIdFromRoute();

        int count = await dbContext.ProductRecords
            .Where(r => r.StorageId == storageId && r.Count >= min)
            .CountAsync();

        return Results.Json(new { Count = count });
    }

    private static async Task<IResult> GetStorageHistory(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "storageId")] string _
    ) {
        var storageId = httpContext.GetStorageIdFromRoute();

        var history = await dbContext.ProductCountChangedRecords
            .Where(r => r.StorageId == storageId)
            .OrderByDescending(r => r.DateTime)
            .ToListAsync();

        var data = history
            .Select(StorageProductCountChangeResponse.FromEntity)
            .ToArray();
        return Results.Json(new { Changes = data });
    }

    private static async Task<IResult> AddProductToStorage(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "storageId")] string _
    ) {
        var storageId = httpContext.GetStorageIdFromRoute();
        var request = httpContext.GetValidatedRequest<ChangeProductCountRequest>();

        var storage = await dbContext.Storages
            .WithProductRecords()
            .WithProductCountHistory()
            .FirstOrDefaultAsync(s => s.Id == storageId);

        if (storage is null)
            return CustomResults.ErrorResponse(ErrFactory.NotFound("Storage not found"));

        var count = storage.AddProduct(new ProductId(new(request.ProductId)), request.Count);

        await dbContext.SaveChangesAsync();
        return Results.Json(new { NewCount = count });
    }

    private static async Task<IResult> RemoveProductFromStorage(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "storageId")] string _
    ) {
        var storageId = httpContext.GetStorageIdFromRoute();
        var request = httpContext.GetValidatedRequest<ChangeProductCountRequest>();

        var storage = await dbContext.Storages
            .WithProductRecords()
            .WithProductCountHistory()
            .FirstOrDefaultAsync(s => s.Id == storageId);

        if (storage is null) {
            return CustomResults.ErrorResponse(ErrPresets.StorageNotFound(storageId));
        }

        var result = storage.RemoveProduct(new ProductId(new(request.ProductId)), request.Count);
        if (result.IsErr(out var err)) {
            return CustomResults.ErrorResponse(err);
        }

        await dbContext.SaveChangesAsync();
        return Results.Json(new { NewCount = result.AsSuccess() });
    }

    private static async Task<IResult> DeleteStorage(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "storageId")] string _
    ) {
        var storageId = httpContext.GetStorageIdFromRoute();

        var hasProducts = await dbContext.ProductRecords
            .AnyAsync(r => r.StorageId == storageId && r.Count > 0);

        if (hasProducts) {
            return CustomResults.ErrorResponse(ErrFactory.Conflict(
                "Cannot delete storage that still contains products"
            ));
        }

        var storage = await dbContext.Storages.FindAsync(storageId);
        if (storage is null) {
            return CustomResults.ErrorResponse(ErrPresets.StorageNotFound(storageId));
        }

        dbContext.Storages.Remove(storage);
        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }

    private static async Task<IResult> UpdateProductAccessLevel(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "storageId")] string _
    ) {
        var storageId = httpContext.GetStorageIdFromRoute();
        var request = httpContext.GetValidatedRequest<UpdateStorageProductAccessLevelRequest>();

        var storage = await dbContext.Storages.FindAsync(storageId);
        if (storage is null) {
            return CustomResults.ErrorResponse(ErrPresets.StorageNotFound(storageId));
        }

        storage.SetProductsAccessLevel(request.AccessLevel);

        await dbContext.SaveChangesAsync();
        return Results.Json(new { NewAccessLevel = storage.ProductAccessLevel });
    }
}