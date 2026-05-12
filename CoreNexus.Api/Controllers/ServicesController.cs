using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoreNexus.Domain.Interfaces;
using CoreNexus.Api.Filters;

namespace CoreNexus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // Solo usuarios con el Token JWT generado en Auth/Login pueden entrar
public class ServicesController : ControllerBase
{
    private readonly ISecureVaultService _vaultService;

    public ServicesController(ISecureVaultService vaultService)
    {
        _vaultService = vaultService;
    }

    /// <summary>
    /// Recupera y muestra el contenido del contexto de seguridad almacenado en la cookie.
    /// </summary>
    [HttpPost("decode-welcome")]
    [ValidateUserContext] // El filtro valida la cookie y descifra el contenido antes de entrar aquí
    public IActionResult DecodeWelcome()
    {
        // El filtro ValidateUserContext ya hizo el trabajo pesado y guardó el resultado en HttpContext.Items
        var decryptedMessage = HttpContext.Items["UserContext"] as string;

        // Si llegamos aquí, es porque la cookie existía y era válida.
        return Ok(new
        {
            Status = "Éxito: Contexto Validado",
            OriginalContent = decryptedMessage,
            Source = "Información recuperada directamente de la Cookie Segura (HttpOnly)",
            Note = "El acceso fue concedido tras validar el JWT y el sobre sellado de la Cookie."
        });
    }
}
