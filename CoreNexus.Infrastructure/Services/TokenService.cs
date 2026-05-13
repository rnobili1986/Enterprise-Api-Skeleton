using CoreNexus.Application.Model;
using CoreNexus.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CoreNexus.Infrastructure.Services;

/// <summary>
/// Servicio encargado de generar tokens JWT utilizados
/// para autenticación y autorización de usuarios.
/// </summary>
/// <remarks>
/// Los tokens son firmados utilizando HMAC SHA256
/// con una clave secreta configurada en la aplicación.
/// </remarks>
public class TokenService : ITokenService
{
    /// <summary>
    /// Configuración utilizada para la generación de JWT.
    /// </summary>
    private readonly JwtSettings _settings;

    /// <summary>
    /// Logger utilizado para registrar eventos y trazabilidad
    /// relacionados con la generación de tokens.
    /// </summary>
    private readonly ILogger<TokenService> _logger;

    /// <summary>
    /// Inicializa una nueva instancia del servicio de tokens.
    /// </summary>
    /// <param name="jwtOptions">
    /// Opciones de configuración JWT inyectadas desde la aplicación.
    /// </param>
    /// <param name="logger">
    /// Servicio de logging utilizado para observabilidad.
    /// </param>
    /// <exception cref="InvalidOperationException">
    /// Se lanza cuando la clave secreta JWT no está configurada.
    /// </exception>
    public TokenService(
        IOptions<JwtSettings> jwtOptions,
        ILogger<TokenService> logger)
    {
        _settings = jwtOptions.Value;
        _logger = logger;

        // Validación temprana para evitar iniciar el servicio
        // sin una clave de firma configurada.
        if (string.IsNullOrEmpty(_settings.Secret))
            throw new InvalidOperationException("JWT Secret is missing in configuration.");
    }

    /// <summary>
    /// Genera un token JWT firmado para un usuario autenticado.
    /// </summary>
    /// <param name="username">
    /// Nombre de usuario asociado al token.
    /// </param>
    /// <param name="role">
    /// Rol o perfil de autorización del usuario.
    /// </param>
    /// <returns>
    /// Token JWT serializado en formato string.
    /// </returns>
    public string GenerateToken(string username, string role)
    {
        // Registro de trazabilidad para auditoría y monitoreo.
        _logger.LogInformation(
            "Generating JWT token for user {Username} with role {Role}",
            username,
            role);

        // Construcción de la clave de firma simétrica.
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.Secret));

        // Credenciales utilizadas para firmar el token.
        var creds = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        // Claims incluidas dentro del JWT.
        var claims = new List<Claim>
        {
            // Identificador principal del usuario.
            new Claim(JwtRegisteredClaimNames.Sub, username),

            // Identificador único del token.
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),

            // Rol del usuario para autorización.
            new Claim(ClaimTypes.Role, role),

            // Fecha de generación del token.
            new Claim(
                "GeneratedAt",
                DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"))
        };

        // Definición de propiedades y configuración del token.
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),

            // Expiración configurable.
            // Si no se define duración, se utiliza 60 minutos por defecto.
            Expires = DateTime.UtcNow.AddMinutes(
                _settings.DurationInMinutes == 0
                    ? 60
                    : _settings.DurationInMinutes),

            // Emisor válido del token.
            Issuer = _settings.Issuer,

            // Audiencia válida del token.
            Audience = _settings.Audience,

            // Credenciales de firma.
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        // Creación del token JWT.
        var token = tokenHandler.CreateToken(tokenDescriptor);

        // Registro exitoso de generación de token.
        _logger.LogInformation(
            "JWT token generated successfully for user {Username}",
            username);

        // Serialización del token a string.
        return tokenHandler.WriteToken(token);
    }
}