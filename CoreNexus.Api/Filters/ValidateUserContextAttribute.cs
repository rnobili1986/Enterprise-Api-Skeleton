using CoreNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CoreNexus.Api.Filters;

public class ValidateUserContextAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var vaultService = context.HttpContext.RequestServices.GetService<ISecureVaultService>();

        // 1. Intentar obtener la cookie
        if (!context.HttpContext.Request.Cookies.TryGetValue("X-User-Context", out var encryptedContext))
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Falta el contexto de seguridad (Cookie)." });
            return;
        }

        try
        {
            // 2. Intentar descifrar el contenido para validar que es legítimo
            // Si el IV fue manipulado o la llave no coincide, lanzará excepción
            var decryptedMessage = vaultService.Unprotect(encryptedContext);

            // 3. Opcional: Podrías guardar el mensaje descifrado en HttpContext 
            // para que el controlador lo use sin tener que descifrarlo otra vez
            context.HttpContext.Items["UserContext"] = decryptedMessage;
        }
        catch
        {
            context.Result = new UnauthorizedObjectResult(new { message = "Contexto de seguridad inválido o expirado." });
        }
    }
}