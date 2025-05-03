using ApiShared;
using Microsoft.EntityFrameworkCore;
using StorageService.Api.contracts;
using StorageService.Api.extensions;
using StorageService.Api.services;
using StorageService.Domain.entities;
using StorageService.Domain.errs;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints;

internal static class CompanyAdminsHandlers
{
    internal static IEndpointRouteBuilder MapCompanyAdminsHandlers(this RouteGroupBuilder endpoints) {
        // endpoints.MapGet("/me", GetAdminInfo);
        endpoints.MapPost("/register", RegisterAdmin)
            .WithRequestValidation<RegisterCompanyAdminRequest>();
        // endpoints.MapPost("/login", LoginAdmin)
        //     .WithRequestValidation<RegisterCompanyAdminRequest>();
        return endpoints;
    }

    private static async Task<IResult> RegisterAdmin(
        HttpContext httpContext, AppDbContext dbContext, PasswordHasher passwordHasher
    ) {
        var request = httpContext.GetValidatedRequest<RegisterCompanyAdminRequest>();

        CompanyAdmin? existingAdmin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Name == request.Name);
        if (existingAdmin is not null) {
            return CustomResults.ErrorResponse(ErrFactory.Conflict("Company admin with this name already exists"));
        }

        string passwordHash = passwordHasher.HashPassword(request.Password);
        var newAdminRes = CompanyAdmin.CreateNew(request.Name, passwordHash);
        if (newAdminRes.IsErr(out var err)) {
            return CustomResults.ErrorResponse(err);
        }

        CompanyAdmin admin = newAdminRes.AsSuccess();

        await dbContext.Admins.AddAsync(admin);
        await dbContext.SaveChangesAsync();

        return Results.Ok();
    }
}