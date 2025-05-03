using Microsoft.EntityFrameworkCore;
using StorageService.Domain.date_time_provider;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.extensions;

public static class BuilderServicesConfigurationExtensions
{
    internal static IServiceCollection AddAuthConfig(
        this IServiceCollection services, IConfiguration configuration
    ) {
        var jwtTokenConfig = configuration.GetSection("JwtTokenConfig").Get<JwtTokenConfig>();
        if (jwtTokenConfig is null) {
            throw new Exception("JWT token config not configured");
        }

        services.AddSingleton(jwtTokenConfig);
        return services;
    }
    internal  static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration) {
        string dbConnectionString = configuration.GetConnectionString("StorageServiceDb")
                                    ?? throw new Exception("Database connection string is not provided.");
        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dbConnectionString));

        return services;
    }

    internal  static IServiceCollection AddDateTimeService(this IServiceCollection services) {
        services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
        return services;
    }

}