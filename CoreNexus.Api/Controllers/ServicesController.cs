using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CoreNexus.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize] // <--- ¡ESTO ES LA CLAVE! Bloquea el acceso a cualquiera sin token
public class ServicesController : ControllerBase
{
    [HttpGet("data")]
    public IActionResult GetProtectedData()
    {
        return Ok(new
        {
            Status = "Success",
            Data = "Este es un dato ultra secreto que solo tú puedes ver."
        });
    }
}
