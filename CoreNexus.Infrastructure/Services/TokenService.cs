using CoreNexus.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoreNexus.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _config;

    public TokenService(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(string username, string role)
    {
        // 1. EXTRAER CONFIGURACIÓN
        // Es vital que estos valores existan en tu appsettings.json
        var secretKey = _config["JwtSettings:Secret"]
            ?? throw new InvalidOperationException("JWT Secret no encontrado en la configuración.");

        var issuer = _config["JwtSettings:Issuer"];
        var audience = _config["JwtSettings:Audience"];
        var duration = double.Parse(_config["JwtSettings:DurationInMinutes"] ?? "60");

        // 2. CONFIGURAR LA LLAVE CRIPTOGRÁFICA
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // 3. DEFINIR LOS CLAIMS (La identidad del usuario)
        // Usamos Claims estándar para asegurar compatibilidad con cualquier cliente (Frontend/IA)
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim("GeneratedAt", DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
        };

        // 4. CREAR EL DESCRIPTOR DEL TOKEN
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(duration),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = creds
        };

        // 5. GENERAR EL TOKEN
        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}