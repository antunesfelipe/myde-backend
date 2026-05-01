using Microsoft.EntityFrameworkCore;
using MydeApi.Data;
using MydeApi.Models;

namespace MydeApi.Modules.Proposals;

public class ProposalRepository : IProposalRepository
{
    private readonly AppDbContext _context;
    
    public ProposalRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<Proposal?> GetByIdAsync(Guid id, Guid tenantId)
    {
        return await _context.Proposals
            .Include(p => p.Client)
            .Where(p => p.Id == id && p.TenantId == tenantId)
            .FirstOrDefaultAsync();
    }
    
    public async Task<Proposal?> GetByProtocolAsync(string protocol)
    {
        return await _context.Proposals
            .FirstOrDefaultAsync(p => p.ExternalProtocol == protocol);
    }
    
    public async Task<(List<Proposal> Items, int Total)> GetAllAsync(Guid tenantId, int page, int pageSize)
    {
        var query = _context.Proposals
            .Include(p => p.Client)
            .Where(p => p.TenantId == tenantId);
        
        var total = await query.CountAsync();
        
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, total);
    }
    
    public async Task<Proposal> CreateAsync(Proposal proposal)
    {
        _context.Proposals.Add(proposal);
        await _context.SaveChangesAsync();
        return proposal;
    }
    
    public async Task<Proposal> UpdateAsync(Proposal proposal)
    {
        proposal.UpdatedAt = DateTime.UtcNow;
        _context.Proposals.Update(proposal);
        await _context.SaveChangesAsync();
        return proposal;
    }
}
