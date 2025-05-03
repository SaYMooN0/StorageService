using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageService.Api.contracts.products;
using StorageService.Api.extensions;
using StorageService.Domain.common;
using StorageService.Domain.entities;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints;

internal static class SpecificProductHandlers
{
    internal static IEndpointRouteBuilder MapSpecificProductHandlers(this RouteGroupBuilder endpoints) {
        endpoints.MapGet("/", GetProductInfo);
        endpoints.MapGet("/find-in-storages", FindInStorages);

        return endpoints;
    }

    private static async Task<IResult> GetProductInfo(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "productId")] string _
        //по причине идиотского поведения Swashbuckle который не дает сгенерить нормальнью OpenApi спецификацию 
        //с productId в качестве аргумента если "он не указан в параметрах самого хэндлера"
        //даже .WithOpenApi бессилен. по словами чат гпт такое поведение поменять нельзя... :) 
    ) {
        ProductId productId = httpContext.GetProductIdFromRoute();

        Product? product = await dbContext.Products.FindAsync(productId);
        if (product is null) {
            return CustomResults.ErrorResponse(ErrPresets.ProductNotFound(productId));
        }

        return Results.Json(ProductDataResponse.FromProduct(product));
    }

    private static async Task<IResult> FindInStorages(
        HttpContext httpContext,
        AppDbContext dbContext,
        [FromRoute(Name = "productId")] string _,
        [FromQuery] uint minCount = 0
    ) {
        ProductId productId = httpContext.GetProductIdFromRoute();

        bool productExists = await dbContext.Products.AnyAsync(p => p.Id == productId);
        if (!productExists) {
            return CustomResults.ErrorResponse(ErrPresets.ProductNotFound(productId));
        }

        var records = await dbContext.ProductRecords
            .Where(r => r.ProductId == productId && r.Count > minCount)
            .ToListAsync();
        return Results.Json(ProductInStoragesResponse.FromProductRecords(records));
    }
}