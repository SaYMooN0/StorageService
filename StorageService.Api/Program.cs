using Microsoft.OpenApi.Models;
using StorageService.Api.endpoints;
using StorageService.Api.extensions;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api;

public class Program
{
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services
            .AddAuthRelatedServices(builder.Configuration)
            .AddPersistence(builder.Configuration)
            .AddDateTimeService();

        builder.Services.AddProblemDetails();
        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
            app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "StorageService.Api"));
        }
        else {
            app.UseHttpsRedirection();
        }

        // using (var serviceScope = app.Services.CreateScope()) {
        //     var db = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
        //     db.Database.EnsureDeleted();
        //     db.Database.EnsureCreated();
        //
        //     db.SaveChanges();
        // }

        MapHandlers(app);
        app.Run();
    }

    private static void MapHandlers(WebApplication app) {
        app.MapGroup("/admins/").MapCompanyAdminsHandlers();
        
        app.MapGroup("/storages/").MapStoragesHandlers();
        app.MapGroup("/storages/{storageId}/").MapSpecificStorageHandlers();
        
        app.MapGroup("/products/").MapProductsHandlers();
        app.MapGroup("/products/{productId}/").MapSpecificProductHandlers();
    }
}