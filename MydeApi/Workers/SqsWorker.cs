using Amazon.SQS;
using Amazon.SQS.Model;
using System.Text.Json;
using MydeApi.Services;
using MydeApi.Modules.Proposals;

namespace MydeApi.Workers;

public class SqsWorker : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IAmazonSQS _sqsClient;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SqsWorker> _logger;
    
    public SqsWorker(
        IServiceProvider serviceProvider,
        IAmazonSQS sqsClient,
        IConfiguration configuration,
        ILogger<SqsWorker> logger)
    {
        _serviceProvider = serviceProvider;
        _sqsClient = sqsClient;
        _configuration = configuration;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SQS Worker iniciado");
        
        var queueUrl = _configuration["AWS:SQS:QueueUrl"];
        
        if (string.IsNullOrEmpty(queueUrl))
        {
            _logger.LogWarning("Queue URL não configurada. Worker não iniciará.");
            return;
        }
        
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var request = new ReceiveMessageRequest
                {
                    QueueUrl = queueUrl,
                    MaxNumberOfMessages = 1,
                    WaitTimeSeconds = 20 // Long polling
                };
                
                var response = await _sqsClient.ReceiveMessageAsync(request, stoppingToken);
                
                foreach (var message in response.Messages)
                {
                    await ProcessMessageAsync(message, queueUrl, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar mensagens SQS");
                await Task.Delay(5000, stoppingToken); // Espera 5s antes de tentar novamente
            }
        }
        
        _logger.LogInformation("SQS Worker finalizado");
    }
    
    private async Task ProcessMessageAsync(Message message, string queueUrl, CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Processando mensagem: {MessageId}", message.MessageId);
            
            var job = JsonSerializer.Deserialize<SqsJob>(message.Body);
            
            if (job == null)
            {
                _logger.LogWarning("Mensagem inválida: {Body}", message.Body);
                await DeleteMessageAsync(message, queueUrl);
                return;
            }
            
            // Criar scope para injeção de dependência
            using var scope = _serviceProvider.CreateScope();
            var proposalRepository = scope.ServiceProvider.GetRequiredService<IProposalRepository>();
            var mockBankService = scope.ServiceProvider.GetRequiredService<MockBankService>();
            
            // Buscar proposta
            var proposal = await proposalRepository.GetByIdAsync(job.ProposalId, Guid.Empty);
            
            if (proposal == null)
            {
                _logger.LogWarning("Proposta não encontrada: {ProposalId}", job.ProposalId);
                await DeleteMessageAsync(message, queueUrl);
                return;
            }
            
            // Atualizar status para "processing"
            proposal.Status = "processing";
            await proposalRepository.UpdateAsync(proposal);
            
            // Chamar banco mock
            string protocol;
            
            if (job.Type == "simulate")
            {
                protocol = await mockBankService.SimulateAsync(
                    job.Cpf ?? "",
                    job.Amount ?? 0,
                    job.Installments ?? 0
                );
            }
            else // submit
            {
                var clientData = new Dictionary<string, object>
                {
                    { "name", proposal.Client.Name },
                    { "cpf", proposal.Client.Cpf }
                };
                
                protocol = await mockBankService.SubmitAsync(job.Protocol ?? "", clientData);
            }
            
            // Salvar protocolo
            proposal.ExternalProtocol = protocol;
            await proposalRepository.UpdateAsync(proposal);
            
            _logger.LogInformation("Proposta {ProposalId} processada com protocolo {Protocol}", 
                proposal.Id, protocol);
            
            // Deletar mensagem da fila
            await DeleteMessageAsync(message, queueUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao processar job: {MessageId}", message.MessageId);
            // Não deleta a mensagem - SQS vai reenviar automaticamente
        }
    }
    
    private async Task DeleteMessageAsync(Message message, string queueUrl)
    {
        await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
        {
            QueueUrl = queueUrl,
            ReceiptHandle = message.ReceiptHandle
        });
    }
}

public class SqsJob
{
    public Guid ProposalId { get; set; }
    public string? Cpf { get; set; }
    public decimal? Amount { get; set; }
    public int? Installments { get; set; }
    public string? Protocol { get; set; }
    public string Type { get; set; } = string.Empty;
}
