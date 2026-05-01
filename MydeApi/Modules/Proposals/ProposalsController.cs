using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace MydeApi.Modules.Proposals;

[ApiController]
[Route("api/proposals")]
[Authorize]
public class ProposalsController : ControllerBase
{
    private readonly IProposalService _proposalService;

    public ProposalsController(IProposalService proposalService)
    {
        _proposalService = proposalService;
    }

    private Guid GetTenantId()
    {
        var tenantIdClaim = User.FindFirst("tenant_id")?.Value;
        return Guid.Parse(tenantIdClaim ?? throw new UnauthorizedAccessException("Tenant ID não encontrado"));
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst("user_id")?.Value;
        return Guid.Parse(userIdClaim ?? throw new UnauthorizedAccessException("User ID não encontrado"));
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        var tenantId = GetTenantId();
        var proposals = await _proposalService.GetAllAsync(tenantId, page, pageSize);
        return Ok(proposals);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tenantId = GetTenantId();
        var proposal = await _proposalService.GetByIdAsync(id, tenantId);
        
        if (proposal == null)
            return NotFound();
        
        return Ok(proposal);
    }

    [HttpPost("simulate")]
    public async Task<IActionResult> Simulate([FromBody] SimulateProposalRequest request)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        
        var proposal = await _proposalService.SimulateAsync(request, tenantId, userId);
        return Accepted(proposal);
    }

    [HttpPost("{id}/submit")]
    public async Task<IActionResult> Submit(Guid id)
    {
        var tenantId = GetTenantId();
        
        var proposal = await _proposalService.SubmitAsync(id, tenantId);
        
        if (proposal == null)
            return NotFound();
        
        return Ok(proposal);
    }
}
