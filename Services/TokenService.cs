using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Models;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(Cliente cliente)
    {
        var secretKey = _configuration["Jwt:Key"] ?? "ClaveSuperSecretaParaApiDeClientes1234567890";
        var issuer = _configuration["Jwt:Issuer"] ?? "APIClientes";
        var audience = _configuration["Jwt:Audience"] ?? "APIClientesClient";

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, cliente.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, cliente.Id.ToString()),
            new Claim(ClaimTypes.Name, cliente.Nombre),
            new Claim(ClaimTypes.Email, cliente.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
