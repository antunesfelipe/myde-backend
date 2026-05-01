
namespace MydeApi.Modules.Clients;

public interface IClientService
{
    Task<ClientResponse?> GetByIdAsync(Guid id, Guid tenantId);
    Task<PaginatedResponse<ClientResponse>> GetAllAsync(Guid tenantId, int page, int pageSize);
    Task<ClientResponse> CreateAsync(CreateClientRequest request, Guid tenantId, Guid userId);
    Task<ClientResponse?> UpdateAsync(Guid id, UpdateClientRequest request, Guid tenantId);
}

