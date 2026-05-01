using MydeApi.Models;
using MydeApi.Services;
using MydeApi.Modules.Clients;

namespace MydeApi.Modules.Proposals;

public class ProposalService : IProposalService
{
    private readonly IProposalRepository _proposalRepository;
    private readonly IClientRepository _clientRepository;
    private readonly SqsService _sqsService;
    
    public ProposalService(
        IProposalRepository proposalRepository,
        IClientRepository clientRepository,
        SqsService sqsService)
    {
        _proposalRepository = proposalRepository;
        _clientRepository = clientRepository;
        _sqsService = sqsService;
    }
    
    public async Task<ProposalResponse?> GetByIdAsync(Guid id, Guid tenantId)
    {
        var proposal = await _proposalRepository.GetByIdAsync(id, tenantId);
        
        if (proposal == null)
        {
            return null;
        }
        
        return MapToResponse(proposal);
    }
    
    public async Task<PaginatedResponse<ProposalResponse>> GetAllAsync(Guid tenantId, int page, int pageSize)
    {
        var (items, total) = await _proposalRepository.GetAllAsync(tenantId, page, pageSize);
        
        return new PaginatedResponse<ProposalResponse>
        {
            Items = items.Select(MapToResponse).ToList(),
            Total = total,
            Page = page,
            PageSize = pageSize
        };
    }
    
    public async Task<ProposalResponse> SimulateAsync(SimulateProposalRequest request, Guid tenantId, Guid userId)
    {
        // Validar se cliente existe
        var client = await _clientRepository.GetByIdAsync(request.ClientId, tenantId);
        
        if (client == null)
        {
            throw new InvalidOperationException("Cliente não encontrado");
        }
        
        // Criar proposta com status "pending"
        var proposal = new Proposal
        {
            TenantId = tenantId,
            ClientId = request.ClientId,
            Type = "simulacao",
            Amount = request.Amount,
            Installments = request.Installments,
            Status = "pending",
            CreatedBy = userId
        };
        
        var created = await _proposalRepository.CreateAsync(proposal);
        
        // Enfileirar job na fila SQS
        await _sqsService.SendMessageAsync(new
        {
            ProposalId = created.Id,
            Cpf = client.Cpf,
            Amount = created.Amount,
            Installments = created.Installments,
            Type = "simulate"
        });
        
        return MapToResponse(created);
    }
    
    public async Task<ProposalResponse?> SubmitAsync(Guid id, Guid tenantId)
    {
        var proposal = await _proposalRepository.GetByIdAsync(id, tenantId);
        
        if (proposal == null)
        {
            return null;
        }
        
        // Validar se pode submeter (deve estar simulada)
        if (proposal.Status != "simulated")
        {
            throw new InvalidOperationException("Proposta deve estar simulada para ser submetida");
        }
        
        // Atualizar status para pending
        proposal.Status = "pending";
        proposal.Type = "proposta";
        
        await _proposalRepository.UpdateAsync(proposal);
        
        // Enfileirar job
        await _sqsService.SendMessageAsync(new
        {
            ProposalId = proposal.Id,
            Protocol = proposal.ExternalProtocol,
            Type = "submit"
        });
        
        return MapToResponse(proposal);
    }
    
    private static ProposalResponse MapToResponse(Proposal proposal)
    {
        return new ProposalResponse
        {
            Id = proposal.Id,
            ClientId = proposal.ClientId,
            ExternalProtocol = proposal.ExternalProtocol,
            Type = proposal.Type,
            Amount = proposal.Amount,
            Installments = proposal.Installments,
            InterestRate = proposal.InterestRate,
            InstallmentValue = proposal.InstallmentValue,
            Status = proposal.Status,
            BankResponse = proposal.BankResponse,
            CreatedAt = proposal.CreatedAt,
            UpdatedAt = proposal.UpdatedAt
        };
    }
}
