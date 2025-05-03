using StorageService.Api.endpoints;
using StorageService.Api.extensions;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api;

public class Program
{
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddOpenApi();
        builder.Services
            .AddAuthRelatedServices(builder.Configuration)
            .AddPersistence(builder.Configuration)
            .AddDateTimeService();

        var app = builder.Build();

        if (app.Environment.IsDevelopment()) {
            app.MapOpenApi();
        }
        else {
            app.UseHttpsRedirection();
        }

        using (var serviceScope = app.Services.CreateScope()) {
            var db = serviceScope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
            // db.AppUsers.Add(new(new(new("0196405c-0c03-7520-8da6-d17cdc334ba7"))));
            db.SaveChanges();
        }

        MapHandlers(app);
        app.Run();
    }

    private static void MapHandlers(WebApplication app) {
        app.MapGroup("/admins").MapCompanyAdminsHandlers();
        app.MapGroup("/storages").MapStoragesHandlers();
        app.MapGroup("/storages/{storageId}").MapSpecificStorageHandlers();
    }
}