
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MydeApi.Models;

[Table("clients")]
public class Client
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
    [MaxLength(200)]
    [Column("name")]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(11)]
    [Column("cpf")]
    public string Cpf { get; set; } = string.Empty;
    
    [Required]
    [Column("birth_date")]
    public DateTime BirthDate { get; set; }
    
    [MaxLength(20)]
    [Column("phone")]
    public string Phone { get; set; } = string.Empty;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    [Required]
    [Column("created_by")]
    public Guid CreatedBy { get; set; }
    
    [ForeignKey("CreatedBy")]
    public User Creator { get; set; } = null!;
    
    // Relacionamentos
    public ICollection<Proposal> Proposals { get; set; } = new List<Proposal>();
}
