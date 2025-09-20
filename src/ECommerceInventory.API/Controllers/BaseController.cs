using ECommerceInventory.Application.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace ECommerceInventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseController : ControllerBase
{
    protected IActionResult HandleException(Exception ex, string operation = "operation")
    {
        return ex switch
        {
            ConflictException => Conflict(new { message = ex.Message }),
            KeyNotFoundException => NotFound(new { message = ex.Message }),
            InvalidOperationException => BadRequest(new { message = ex.Message }),
            UnauthorizedAccessException => Unauthorized(new { message = ex.Message }),
            _ => StatusCode(500, new { message = $"An error occurred during {operation}", details = ex.Message })
        };
    }
}