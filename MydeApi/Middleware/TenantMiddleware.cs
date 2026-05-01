namespace MydeApi.Middleware;

public class TenantMiddleware
{
    private readonly RequestDelegate _next;
    
    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        // Extrair tenant_id do token JWT (já foi validado pelo JwtMiddleware)
        var tenantId = context.Items["TenantId"]?.ToString();
        
        if (!string.IsNullOrEmpty(tenantId))
        {
            // Armazenar no HttpContext para uso posterior
            context.Items["CurrentTenantId"] = tenantId;
        }
        
        await _next(context);
    }
}
