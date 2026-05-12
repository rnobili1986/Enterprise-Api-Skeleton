using CoreNexus.Application.DTOs;
using CoreNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CoreNexus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly ISecureVaultService _vaultService;

    public AuthController(ITokenService tokenService, ISecureVaultService vaultService)
    {
        _tokenService = tokenService;
        _vaultService = vaultService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        // 1. Simulación de validación (Aquí luego podrías conectar a una DB)
        if (request.Username == "admin" && request.Password == "12345")
        {
            // 2. Generamos el Token JWT (Identidad)
            var token = _tokenService.GenerateToken(request.Username, "Administrator");

            // 3. Generamos el Mensaje de Bienvenida con tu DOBLE ENCRIPTACIÓN (Privacidad)
            var rawMessage = $"Bienvenido {request.Username}. Conexión segura establecida.";
            var secureMessage = _vaultService.Protect(rawMessage);

            // 4. AGREGAR LA COOKIE DE SEGURIDAD
            // Esta cookie no es accesible por JavaScript (XSS Protection)
            Response.Cookies.Append("X-User-Context", secureMessage, new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Solo viaja por HTTPS
                SameSite = SameSiteMode.Strict, // Previene CSRF
                Expires = DateTime.UtcNow.AddMinutes(60)
            });

            return Ok(new AuthResponse(
                Token: token,
                EncryptedWelcomeMessage: secureMessage,
                Expiration: DateTime.UtcNow.AddMinutes(60)
            ));
        }

        return Unauthorized("Usuario o contraseña incorrectos.");
    }
}