using MydeApi.Models;

namespace MydeApi.Modules.Proposals;

public interface IProposalRepository
{
    Task<Proposal?> GetByIdAsync(Guid id, Guid tenantId);
    Task<Proposal?> GetByProtocolAsync(string protocol);
    Task<(List<Proposal> Items, int Total)> GetAllAsync(Guid tenantId, int page, int pageSize);
    Task<Proposal> CreateAsync(Proposal proposal);
    Task<Proposal> UpdateAsync(Proposal proposal);
}
