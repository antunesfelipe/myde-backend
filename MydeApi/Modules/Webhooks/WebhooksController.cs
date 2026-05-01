using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using MydeApi.Modules.Proposals;

namespace MydeApi.Modules.Webhooks;

[ApiController]
[Route("api/webhooks")]
public class WebhooksController : ControllerBase
{
    private readonly IProposalRepository _proposalRepository;
    private readonly ILogger<WebhooksController> _logger;
    
    public WebhooksController(
        IProposalRepository proposalRepository,
        ILogger<WebhooksController> logger)
    {
        _proposalRepository = proposalRepository;
        _logger = logger;
    }
    
    /// <summary>
    /// Receber callback do banco mock
    /// </summary>
    [HttpPost("bank-callback")]
    public async Task<IActionResult> BankCallback([FromBody] BankCallbackRequest request)
    {
        _logger.LogInformation("Webhook recebido: {Protocol} - {Event} - {Status}", 
            request.Protocol, request.Event, request.Status);
        
        // Buscar proposta pelo protocolo
        var proposal = await _proposalRepository.GetByProtocolAsync(request.Protocol);
        
        if (proposal == null)
        {
            _logger.LogWarning("Proposta não encontrada para protocolo: {Protocol}", request.Protocol);
            return NotFound(new { message = "Proposta não encontrada" });
        }
        
        // Mapear status do evento
        var newStatus = MapEventToStatus(request.Event, request.Status);
        
        // Atualizar proposta
        proposal.Status = newStatus;
        
        if (request.Data != null)
        {
            proposal.InterestRate = request.Data.InterestRate;
            proposal.InstallmentValue = request.Data.InstallmentValue;
        }
        
        // Salvar resposta completa do banco
        proposal.BankResponse = JsonSerializer.Serialize(request);
        
        await _proposalRepository.UpdateAsync(proposal);
        
        _logger.LogInformation("Proposta {ProposalId} atualizada para status {Status}", 
            proposal.Id, newStatus);
        
        return Ok(new { message = "Webhook processado com sucesso" });
    }
    
    private static string MapEventToStatus(string eventName, string status)
    {
        return eventName switch
        {
            "simulation_completed" => status == "approved" ? "simulated" : "simulation_failed",
            "submission_completed" => status == "approved" ? "approved" : "rejected",
            _ => "processing"
        };
    }
}
