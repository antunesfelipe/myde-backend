using System.Text;
using System.Text.Json;

namespace MydeApi.Services;

public class MockBankService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MockBankService> _logger;
    
    public MockBankService(HttpClient httpClient, ILogger<MockBankService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<string> SimulateAsync(string cpf, decimal amount, int installments)
    {
        var payload = new
        {
            cpf,
            amount,
            installments
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );
        
        try
        {
            var response = await _httpClient.PostAsync("/api/simular", content);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<MockBankResponse>(result);
            
            return data?.Protocol ?? throw new Exception("Protocol not returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao simular no banco mock");
            throw;
        }
    }
    
    public async Task<string> SubmitAsync(string protocol, Dictionary<string, object> clientData)
    {
        var payload = new
        {
            protocol,
            client_data = clientData
        };
        
        var content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );
        
        try
        {
            var response = await _httpClient.PostAsync("/api/incluir", content);
            response.EnsureSuccessStatusCode();
            
            var result = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<MockBankResponse>(result);
            
            return data?.Protocol ?? throw new Exception("Protocol not returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao submeter no banco mock");
            throw;
        }
    }
}

public class MockBankResponse
{
    public string Protocol { get; set; } = string.Empty;
}
