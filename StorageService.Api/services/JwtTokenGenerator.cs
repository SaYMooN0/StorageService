using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using StorageService.Domain.entities;

namespace StorageService.Api.services;

internal class JwtTokenGenerator 
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly TokenValidationParameters _tokenValidationParameters;
    public JwtTokenGenerator(JwtTokenConfig options) {
        _secretKey = options.SecretKey;
        _issuer = options.Issuer;
        _audience = options.Audience;

        _tokenValidationParameters = new TokenValidationParameters {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey))
        };
    }


    public static string GenerateToken(CompanyAdmin user, JwtTokenConfig options) {
        Claim[] claims = [new("adminId", user.Id.ToString())];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            expires: DateTime.UtcNow.AddDays(30),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

