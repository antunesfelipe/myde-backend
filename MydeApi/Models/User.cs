
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MydeApi.Models;

[Table("users")]
public class User
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
    [MaxLength(200)]
    [Column("email")]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;
    
    [Required]
    [MaxLength(50)]
    [Column("role")]
    public string Role { get; set; } = "operator";
    
    [Column("is_active")]
    public bool IsActive { get; set; } = true;
    
    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Relacionamentos
    public ICollection<Client> CreatedClients { get; set; } = new List<Client>();
    public ICollection<Proposal> CreatedProposals { get; set; } = new List<Proposal>();
}

