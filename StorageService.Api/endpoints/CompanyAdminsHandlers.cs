using ApiShared;
using Microsoft.EntityFrameworkCore;
using StorageService.Api.contracts;
using StorageService.Api.contracts.admin;
using StorageService.Api.extensions;
using StorageService.Api.services;
using StorageService.Domain.common;
using StorageService.Domain.entities;
using StorageService.Domain.entities.storage;
using StorageService.Domain.errs;
using StorageService.Infrastructure.persistence;

namespace StorageService.Api.endpoints;

internal static class CompanyAdminsHandlers
{
    internal static IEndpointRouteBuilder MapCompanyAdminsHandlers(this RouteGroupBuilder endpoints) {
        endpoints.MapGet("/me", GetAdminInfo)
            .WithAdminAuthRequired();
        endpoints.MapPost("/register", RegisterAdmin)
            .WithRequestValidation<RegisterCompanyAdminRequest>();
        endpoints.MapPost("/login", LoginAdmin)
            .WithRequestValidation<LoginCompanyAdminRequest>();
        return endpoints;
    }

    private static async Task<IResult> GetAdminInfo(
        HttpContext httpContext, AppDbContext dbContext
    ) {
        CompanyAdminId id = httpContext.GetAuthenticatedAdminId();

        CompanyAdmin? admin = await dbContext.Admins
            .Include(p => EF.Property<List<Storage>>(p, "_storages"))
            .Include(p => EF.Property<List<TransportCompany>>(p, "_transportCompanies"))
            .FirstOrDefaultAsync(a => a.Id == id);
        if (admin is null) {
            return CustomResults.ErrorResponse(ErrFactory.NotFound("Unknown admin account not found"));
        }

        return Results.Json(AdminInfoResponse.FromAdmin(admin));
    }

    private static async Task<IResult> RegisterAdmin(
        HttpContext httpContext,
        AppDbContext dbContext,
        PasswordHasher passwordHasher,
        JwtTokenGenerator jwtTokenGenerator
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

        WriteAuthCookies(httpContext, jwtTokenGenerator, admin);
        return Results.Ok();
    }

    private static async Task<IResult> LoginAdmin(
        HttpContext httpContext, AppDbContext dbContext, PasswordHasher passwordHasher,
        JwtTokenGenerator jwtTokenGenerator
    ) {
        var request = httpContext.GetValidatedRequest<LoginCompanyAdminRequest>();

        CompanyAdmin? admin = await dbContext.Admins.FirstOrDefaultAsync(a => a.Name == request.Name);
        if (admin is null) {
            return CustomResults.ErrorResponse(ErrFactory.NotFound("Company admin with this name does not exist"));
        }

        if (!passwordHasher.VerifyPassword(admin.PasswordHash, request.Password)) {
            return CustomResults.ErrorResponse(ErrFactory.InvalidData("Incorrect password for company admin account"));
        }

        WriteAuthCookies(httpContext, jwtTokenGenerator, admin);
        return Results.Ok();
    }

    private const string AuthCookieName = "_token";

    private static void WriteAuthCookies(
        HttpContext httpContext, JwtTokenGenerator jwtTokenGenerator, CompanyAdmin admin
    ) {
        var token = jwtTokenGenerator.GenerateToken(admin);
        var authCookieOptions = new CookieOptions {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(30)
        };
        httpContext.Response.Cookies.Append(AuthCookieName, token, authCookieOptions);
    }
}