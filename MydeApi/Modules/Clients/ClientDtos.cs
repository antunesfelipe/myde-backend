
using System.ComponentModel.DataAnnotations;

namespace MydeApi.Modules.Clients;

public class CreateClientRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "CPF é obrigatório")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter 11 dígitos")]
    public string Cpf { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Data de nascimento é obrigatória")]
    public DateTime BirthDate { get; set; }
    
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
}

public class UpdateClientRequest
{
    [Required(ErrorMessage = "Nome é obrigatório")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
    
    [MaxLength(20)]
    public string Phone { get; set; } = string.Empty;
}

public class ClientResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}
