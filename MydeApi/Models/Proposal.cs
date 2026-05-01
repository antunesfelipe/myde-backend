
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MydeApi.Models;

[Table("proposals")]
public class Proposal
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [Column("tenant_id")]
    public Guid TenantId { get; set; }
    
    [ForeignKey("TenantId")]
    public Tenant Tenant { get; set; } = null!;
    
    [Required]
    [Column("client_id")]
    public Guid ClientId { get; set; }
    
    [ForeignKey("ClientId")]
    public Client Client { get; set; } = null!;
    
    [MaxLength(100)]
    [Column("external_protocol")]
    public string? ExternalProtocol { get; set; }
    
    [Required]
    [MaxLength(50)]
    [Column("type")]
    public string Type { get; set; } = "simulacao";
    
    [Required]
    [Column("amount", TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }
    
    [Required]
    [Column("installments")]
    public int Installments { get; set; }
    
    [Column("interest_rate", TypeName = "decimal(5,2)")]
    public decimal? InterestRate { get; set; }
    
    [Column("installment_value", TypeName = "decimal(18,2)")]
    public decimal? InstallmentValue { get; set; }
    
    [Required]
    [MaxLength(50)]
    [Column("status")]
    public string Status { get; set; } = "pending";
    
    [Column("bank_response", TypeName = "jsonb")]
    public string? BankResponse { get; set; }
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    [Column("created_by")]
    public Guid CreatedBy { get; set; }
    
    [ForeignKey("CreatedBy")]
    public User Creator { get; set; } = null!;
}


