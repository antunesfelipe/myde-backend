using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MydeApi.Middleware;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;
    
    public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        
        if (token != null)
        {
            AttachUserToContext(context, token);
        }
        
        await _next(context);
    }
    
    private void AttachUserToContext(HttpContext context, string token)
    {
        try
        {
            var secret = _configuration["Jwt:Secret"] ?? throw new InvalidOperationException();
            var key = Encoding.UTF8.GetBytes(secret);
            
            var tokenHandler = new JwtSecurityTokenHandler();
            
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);
            
            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = jwtToken.Claims.First(x => x.Type == "user_id").Value;
            var tenantId = jwtToken.Claims.First(x => x.Type == "tenant_id").Value;
            
            context.Items["UserId"] = userId;
            context.Items["TenantId"] = tenantId;
        }
        catch
        {
            // Token inválido - não faz nada, deixa o [Authorize] tratar
        }
    }
}
