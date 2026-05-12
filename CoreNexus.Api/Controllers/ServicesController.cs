using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoreNexus.Domain.Interfaces;

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
    /// Recibe el mensaje cifrado del Login y lo desencripta.
    /// </summary>
    /// <param name="encryptedMessage">El valor de 'EncryptedWelcomeMessage' obtenido en el Login</param>
    [HttpPost("decode-welcome")]
    public IActionResult DecodeWelcome([FromBody] string encryptedMessage)
    {
        if (string.IsNullOrEmpty(encryptedMessage))
            return BadRequest("No se proporcionó ningún mensaje cifrado.");

        // El servicio extrae automáticamente los IVs dinámicos del cuerpo del mensaje
        var decryptedMessage = _vaultService.Unprotect(encryptedMessage);

        if (string.IsNullOrEmpty(decryptedMessage))
            return BadRequest("No se pudo desencriptar el mensaje. ¿Es el formato correcto?");

        return Ok(new
        {
            Status = "Mensaje Recuperado",
            OriginalContent = decryptedMessage,
            Note = "Este mensaje fue descifrado usando las llaves internas y los IVs dinámicos del payload."
        });
    }
}
