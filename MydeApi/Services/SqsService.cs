using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;

namespace MydeApi.Services;

public class SqsService
{
    private readonly IAmazonSQS _sqsClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SqsService> _logger;
    
    public SqsService(
        IAmazonSQS sqsClient, 
        IConfiguration configuration,
        ILogger<SqsService> logger)
    {
        _sqsClient = sqsClient;
        _configuration = configuration;
        _logger = logger;
    }
    
    public async Task SendMessageAsync(object message)
    {
        var queueUrl = _configuration["AWS:SQS:QueueUrl"] 
            ?? throw new InvalidOperationException("Queue URL not configured");
        
        var messageBody = JsonSerializer.Serialize(message);
        
        var request = new SendMessageRequest
        {
            QueueUrl = queueUrl,
            MessageBody = messageBody
        };
        
        try
        {
            var response = await _sqsClient.SendMessageAsync(request);
            _logger.LogInformation("Mensagem enviada para SQS: {MessageId}", response.MessageId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar mensagem para SQS");
            throw;
        }
    }
}
