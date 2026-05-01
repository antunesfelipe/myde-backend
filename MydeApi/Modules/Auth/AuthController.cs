
using Microsoft.AspNetCore.Mvc;

namespace MydeApi.Modules.Auth;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    
    /// <summary>
    /// Login - retorna JWT token
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        
        if (result == null)
        {
            return Unauthorized(new { message = "Email ou senha inválidos" });
        }
        
        return Ok(result);
    }
}

