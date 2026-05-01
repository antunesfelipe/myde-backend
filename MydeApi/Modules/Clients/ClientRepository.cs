
using Microsoft.EntityFrameworkCore;
using MydeApi.Data;
using MydeApi.Models;

namespace MydeApi.Modules.Clients;

public class ClientRepository : IClientRepository
{
    private readonly AppDbContext _context;
    
    public ClientRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Client?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Clients
            .Where(c => c.Id == id && c.TenantId == tenantId)
            .FirstOrDefaultAsync();
    }
    
    public async Task<(List<Client> Items, int Total)> GetAllAsync(Guid tenantId, int page, int pageSize)
    {
        var query = _context.Clients.Where(c => c.TenantId == tenantId);
        
        var total = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, total);
    }
    
    public async Task<Client> CreateAsync(Client client)
    {
        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return client;
    }
    
    public async Task<Client> UpdateAsync(Client client)
    {
        _context.Clients.Update(client);
        await _context.SaveChangesAsync();
        return client;
    }
    
    public async Task<bool> CpfExistsAsync(string cpf, Guid tenantId, Guid? excludeId = null)
    {
        var query = _context.Clients.Where(c => c.Cpf == cpf && c.TenantId == tenantId);
        
        if (excludeId.HasValue)
        {
            query = query.Where(c => c.Id != excludeId.Value);
        }
        
        return await query.AnyAsync();
    }
}

