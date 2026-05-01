
using Microsoft.EntityFrameworkCore;
using MydeApi.Data;
using MydeApi.Services;
using BCrypt.Net;

namespace MydeApi.Modules.Auth;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;
    
    public AuthService(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }
    
    public async Task<LoginResponse?> LoginAsync(LoginRequest request)
    {
        // Buscar usuário pelo email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.IsActive);
        
        if (user == null)
        {
            return null;
        }
        
        // Verificar senha
        bool passwordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
        
        if (!passwordValid)
        {
            return null;
        }
        
        // Gerar token JWT
        var token = _jwtService.GenerateToken(user.Id, user.TenantId, user.Email, user.Role);
        
        return new LoginResponse
        {
            Token = token,
            UserId = user.Id,
            TenantId = user.TenantId,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };
    }
}
