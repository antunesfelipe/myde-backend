
using MydeApi.Models;

namespace MydeApi.Modules.Clients;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, Guid tenantId);
    Task<(List<Client> Items, int Total)> GetAllAsync(Guid tenantId, int page, int pageSize);
    Task<Client> CreateAsync(Client client);
    Task<Client> UpdateAsync(Client client);
    Task<bool> CpfExistsAsync(string cpf, Guid tenantId, Guid? excludeId = null);
}


