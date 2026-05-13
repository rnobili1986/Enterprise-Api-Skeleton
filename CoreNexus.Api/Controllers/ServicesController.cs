using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoreNexus.Domain.Interfaces;
using CoreNexus.Api.Filters;

namespace CoreNexus.Api.Controllers;

/// <summary>
/// Controlador responsable de exponer servicios protegidos
/// que requieren autenticación y validación de contexto seguro.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize] // Requiere un JWT válido para acceder a cualquier endpoint.
public class ServicesController : ControllerBase
{
    /// <summary>
    /// Servicio utilizado para operaciones de protección
    /// y recuperación de información sensible.
    /// </summary>
    private readonly ISecureVaultService _vaultService;

    /// <summary>
    /// Inicializa una nueva instancia del controlador de servicios.
    /// </summary>
    /// <param name="vaultService">
    /// Servicio encargado de cifrado y descifrado de información.
    /// </param>
    public ServicesController(ISecureVaultService vaultService)
    {
        _vaultService = vaultService;
    }

    /// <summary>
    /// Retorna el mensaje descifrado del contexto de seguridad
    /// previamente validado y almacenado en la solicitud.
    /// </summary>
    [HttpPost("decode-welcome")]
    [ValidateUserContext]
    public IActionResult DecodeWelcome()
    {
        // -------------------------------------------------------------
        // RECUPERACIÓN DEL CONTEXTO DESCIFRADO
        // -------------------------------------------------------------
        // El filtro ValidateUserContext ya realizó:
        // - validación de la cookie
        // - verificación de integridad
        // - descifrado del contenido
        //
        // El resultado queda disponible en HttpContext.Items.
        var decryptedMessage =
            HttpContext.Items["UserContext"] as string;

        // -------------------------------------------------------------
        // RESPUESTA SEGURA
        // -------------------------------------------------------------
        // Si la ejecución llegó hasta aquí significa que:
        // - el JWT fue validado correctamente
        // - la cookie segura era legítima
        // - el contenido pudo descifrarse exitosamente
        return Ok(new
        {
            Status = "Éxito: Contexto Validado",
            OriginalContent = decryptedMessage,
            Source = "Información recuperada desde Cookie Segura HttpOnly",
            Note = "El acceso fue concedido tras validar JWT y contexto cifrado."
        });
    }
}