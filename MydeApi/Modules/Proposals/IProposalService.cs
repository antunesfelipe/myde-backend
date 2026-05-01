namespace MydeApi.Modules.Proposals;

public interface IProposalService
{
    Task<ProposalResponse?> GetByIdAsync(Guid id, Guid tenantId);
    Task<PaginatedResponse<ProposalResponse>> GetAllAsync(Guid tenantId, int page, int pageSize);
    Task<ProposalResponse> SimulateAsync(SimulateProposalRequest request, Guid tenantId, Guid userId);
    Task<ProposalResponse?> SubmitAsync(Guid id, Guid tenantId);
}

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
