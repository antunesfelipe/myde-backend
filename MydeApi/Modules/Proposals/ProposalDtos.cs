using System.ComponentModel.DataAnnotations;

namespace MydeApi.Modules.Proposals;

public class SimulateProposalRequest
{
    [Required(ErrorMessage = "ClientId é obrigatório")]
    public Guid ClientId { get; set; }
    
    [Required(ErrorMessage = "Amount é obrigatório")]
    [Range(100, 1000000, ErrorMessage = "Valor deve estar entre 100 e 1.000.000")]
    public decimal Amount { get; set; }
    
    [Required(ErrorMessage = "Installments é obrigatório")]
    [Range(1, 60, ErrorMessage = "Parcelas devem estar entre 1 e 60")]
    public int Installments { get; set; }
}

public class ProposalResponse
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string? ExternalProtocol { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Installments { get; set; }
    public decimal? InterestRate { get; set; }
    public decimal? InstallmentValue { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? BankResponse { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
