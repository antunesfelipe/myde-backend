using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MydeApi.Modules.Clients;
using System.Security.Claims;

namespace MydeApi.Modules.Clients;

[ApiController]
[Route("api/clients")]
[Authorize]
public class ClientsController : ControllerBase
{
    private readonly IClientService _clientService;

    public ClientsController(IClientService clientService)
    {
        _clientService = clientService;
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
        var result = await _clientService.GetAllAsync(tenantId, page, pageSize);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var tenantId = GetTenantId();
        var client = await _clientService.GetByIdAsync(id, tenantId);
        
        if (client == null)
            return NotFound();
        
        return Ok(client);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateClientRequest request)
    {
        var tenantId = GetTenantId();
        var userId = GetUserId();
        
        var client = await _clientService.CreateAsync(request, tenantId, userId);
        return CreatedAtAction(nameof(GetById), new { id = client.Id }, client);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClientRequest request)
    {
        var tenantId = GetTenantId();
        var client = await _clientService.UpdateAsync(id, request, tenantId);
        
        if (client == null)
            return NotFound();
        
        return Ok(client);
    }
}
