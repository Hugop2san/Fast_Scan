using consulta.vulnerabilidade.Application.AgentFix.Handles;

namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Runtime;

public sealed class FixJobWorker : BackgroundService
{
    private readonly FixJobQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<FixJobWorker> _logger;

    public FixJobWorker(
        FixJobQueue queue,
        IServiceScopeFactory scopeFactory,
        ILogger<FixJobWorker> logger)
    {
        _queue = queue;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var jobId = await _queue.DequeueAsync(stoppingToken);
                using var scope = _scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<ProcessFixJobHandler>();
                await handler.HandleAsync(jobId.ToString(), stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao processar fix job.");
            }
        }
    }
}
