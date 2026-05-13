using CoreNexus.Application.DTOs;
using CoreNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoreNexus.Api.Controllers;

/// <summary>
/// Controlador encargado de gestionar operaciones
/// relacionadas con autenticación y generación de sesión.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    /// <summary>
    /// Servicio responsable de generar tokens JWT.
    /// </summary>
    private readonly ITokenService _tokenService;

    /// <summary>
    /// Servicio encargado de proteger y recuperar información sensible mediante cifrado.
    /// </summary>
    private readonly ISecureVaultService _vaultService;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de autenticación.
    /// </summary>
    /// <param name="tokenService">
    /// Servicio utilizado para generar tokens JWT.
    /// </param>
    /// <param name="vaultService">
    /// Servicio utilizado para cifrar información sensible.
    /// </param>
    public AuthController(
        ITokenService tokenService,
        ISecureVaultService vaultService)
    {
        _tokenService = tokenService;
        _vaultService = vaultService;
    }

    /// <summary>
    /// Autentica un usuario y genera un token JWT junto con un contexto de seguridad cifrado.
    /// </summary>
    /// <param name="request">
    /// Credenciales de autenticación enviadas por el cliente.
    /// </param>
    /// <returns>
    /// Información de autenticación incluyendo token JWT, mensaje cifrado y expiración de sesión.
    /// </returns>
    /// <response code="200">
    /// Usuario autenticado correctamente.
    /// </response>
    /// <response code="401">
    /// Usuario o contraseña inválidos.
    /// </response>
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // -----------------------------------------------------------------
        // VALIDACIÓN DE CREDENCIALES
        // -----------------------------------------------------------------
        // Simulación de autenticación.
        // Posteriormente podría integrarse con base de datos, Identity, LDAP, OAuth o proveedores externos.
        if (request.Username == "admin" &&
            request.Password == "12345")
        {
            // -------------------------------------------------------------
            // GENERACIÓN DEL TOKEN JWT
            // -------------------------------------------------------------
            // El token representa la identidad autenticada del usuario.
            var token = _tokenService.GenerateToken(
                request.Username,
                "Administrator");

            // -------------------------------------------------------------
            // GENERACIÓN DEL CONTEXTO DE SEGURIDAD CIFRADO
            // -------------------------------------------------------------
            // Se crea un mensaje privado que posteriormente será almacenado de forma segura en una cookie protegida.
            var rawMessage =
                $"Bienvenido {request.Username}. Conexión segura establecida.";

            var secureMessage =
                _vaultService.Protect(rawMessage);

            // -------------------------------------------------------------
            // CONFIGURACIÓN DE COOKIE SEGURA
            // -------------------------------------------------------------
            // La cookie almacena el contexto cifrado y aplica configuraciones orientadas a seguridad:
            // HttpOnly  -> evita acceso desde JavaScript (XSS)
            // Secure    -> solo transmisión HTTPS
            // SameSite  -> mitigación de ataques CSRF
            // Expires   -> expiración controlada
            Response.Cookies.Append(
                "X-User-Context",
                secureMessage,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(60)
                });

            // -------------------------------------------------------------
            // RESPUESTA DE AUTENTICACIÓN
            // -------------------------------------------------------------
            return Ok(new AuthResponse(
                Token: token,
                EncryptedWelcomeMessage: secureMessage,
                Expiration: DateTime.UtcNow.AddMinutes(60)
            ));
        }

        // Credenciales inválidas.
        return Unauthorized("Usuario o contraseña incorrectos.");
    }
}