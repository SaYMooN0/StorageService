using System.Collections.Immutable;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorageService.Api.contracts.products;
using StorageService.Api.extensions;
using StorageService.Domain.entities;
using StorageService.Domain.errs;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints;

internal static class ProductsHandlers
{
    internal static IEndpointRouteBuilder MapProductsHandlers(this RouteGroupBuilder endpoints) {
        endpoints.MapGet("/", ListProducts);
        endpoints.MapGet("/count", CountProducts);
        endpoints.MapGet("/search-by-name/{searchValue}", SearchProductByName);

        endpoints.MapPost("/initialize-new", InitializeNewProduct)
            .WithAdminAuthRequired()
            .WithRequestValidation<InitializeNewProductRequest>();

        return endpoints;
    }

    private static async Task<IResult> ListProducts(AppDbContext dbContext) {
        var products = await dbContext.Products.ToListAsync();
        return Results.Json(ListProductsResponse.FromProducts(products));
    }
    private static async Task<IResult> CountProducts(AppDbContext dbContext)
    {
        int count = await dbContext.Products.CountAsync();
        return Results.Json(new { Count = count });
    }
    private static async Task<IResult> SearchProductByName(
        [FromRoute] string searchValue,
        AppDbContext dbContext
    ) {
        if (string.IsNullOrWhiteSpace(searchValue)) {
            return CustomResults.ErrorResponse(ErrFactory.NoValue("Please provide a search value"));
        }
        var products = await dbContext.Products
            .Where(p => EF.Functions.ILike(p.Name, $"%{searchValue}%"))
            .ToListAsync();

        return Results.Json(ListProductsResponse.FromProducts(products));
    }

    private static async Task<IResult> InitializeNewProduct(
        HttpContext httpContext,
        AppDbContext dbContext
    ) {
        var request = httpContext.GetValidatedRequest<InitializeNewProductRequest>();
        var props = request.Props.ToImmutableDictionary();

        var result = Product.CreateNew(request.Name, request.Description, props);
        if (result.IsErr(out var err)) {
            return CustomResults.ErrorResponse(err);
        }

        Product product = result.AsSuccess();

        await dbContext.Products.AddAsync(product);
        await dbContext.SaveChangesAsync();

        return CustomResults.Created(new { ProductId = product.Id });
    }
}