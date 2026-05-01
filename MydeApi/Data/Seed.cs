
using MydeApi.Models;
using BCrypt.Net;

namespace MydeApi.Data;

public static class Seed
{
    public static void Initialize(AppDbContext context)
    {
        // Garantir que o banco foi criado
        context.Database.EnsureCreated();
        
        // Se já tem dados, não fazer nada
        if (context.Tenants.Any())
        {
            return;
        }
        
        // Criar Tenant 1
        var tenant1 = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Banco A",
            Document = "12345678000100",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        // Criar Tenant 2
        var tenant2 = new Tenant
        {
            Id = Guid.NewGuid(),
            Name = "Banco B",
            Document = "98765432000199",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        context.Tenants.AddRange(tenant1, tenant2);
        context.SaveChanges();
        
        // Criar Usuário 1 (Banco A)
        var user1 = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenant1.Id,
            Name = "Operador Banco A",
            Email = "operador@bancoa.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
            Role = "operator",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        // Criar Usuário 2 (Banco B)
        var user2 = new User
        {
            Id = Guid.NewGuid(),
            TenantId = tenant2.Id,
            Name = "Operador Banco B",
            Email = "operador@bancob.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("senha123"),
            Role = "operator",
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        
        context.Users.AddRange(user1, user2);
        context.SaveChanges();
    }
}

