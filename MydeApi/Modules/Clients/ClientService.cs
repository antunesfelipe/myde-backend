
using MydeApi.Models;

namespace MydeApi.Modules.Clients;

public class ClientService : IClientService
{
    private readonly IClientRepository _repository;
    
    public ClientService(IClientRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<ClientResponse?> GetByIdAsync(Guid id, Guid tenantId)
    {
        var client = await _repository.GetByIdAsync(id, tenantId);
        
        if (client == null)
        {
            return null;
        }
        
        return MapToResponse(client);
    }
    
    public async Task<PaginatedResponse<ClientResponse>> GetAllAsync(Guid tenantId, int page, int pageSize)
    {
        var (items, total) = await _repository.GetAllAsync(tenantId, page, pageSize);
        
        return new PaginatedResponse<ClientResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }
    
    public async Task<ClientResponse> CreateAsync(CreateClientRequest request, Guid tenantId, Guid userId)
    {
        // Validar se CPF já existe
        var cpfExists = await _repository.CpfExistsAsync(request.Cpf, tenantId);
        
        if (cpfExists)
        {
            throw new InvalidOperationException("CPF já cadastrado neste tenant");
        }
        
        var client = new Client
        {
            TenantId = tenantId,
            Name = request.Name,
            Cpf = request.Cpf,
            BirthDate = request.BirthDate,
            Phone = request.Phone,
            CreatedBy = userId
        };
        
        var created = await _repository.CreateAsync(client);
        
        return MapToResponse(created);
    }
    
    public async Task<ClientResponse?> UpdateAsync(Guid id, UpdateClientRequest request, Guid tenantId)
    {
        var client = await _repository.GetByIdAsync(id, tenantId);
        
        if (client == null)
        {
            return null;
        }
        
        client.Name = request.Name;
        client.Phone = request.Phone;
        
        var updated = await _repository.UpdateAsync(client);
        
        return MapToResponse(updated);
    }
    
    private static ClientResponse MapToResponse(Client client)
    {
        return new ClientResponse
        {
            Id = client.Id,
            Name = client.Name,
            Cpf = client.Cpf,
            BirthDate = client.BirthDate,
            Phone = client.Phone,
            CreatedAt = client.CreatedAt
        };
    }
}
