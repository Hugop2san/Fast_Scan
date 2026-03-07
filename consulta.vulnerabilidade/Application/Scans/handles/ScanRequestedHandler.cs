namespace Consulta.Vulnerabilidade.Application.Scans.Handlers;

using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Domain.Scans;
using consulta.vulnerabilidade.Domain.Scans.Events;

public sealed class ScanRequestedHandler : IEventHandler<ScanRequested>
{
    private readonly IPublicSurfaceExtractor _extractor;
    private readonly IEnumerable<IVulnerabilityAnalyzer> _analyzers;
    private readonly IScanReportRepository _repo;
    private readonly IAiModel? _ai; // opcional
    private readonly IEventBus _bus;

    public ScanRequestedHandler(
        IPublicSurfaceExtractor extractor,
        IEnumerable<IVulnerabilityAnalyzer> analyzers,
        IScanReportRepository repo,
        IEventBus bus,
        IAiModel? ai = null)
    {
        _extractor = extractor;
        _analyzers = analyzers;
        _repo = repo;
        _bus = bus;
        _ai = ai;
    }

    public async Task HandleAsync(ScanRequested @event, CancellationToken ct = default)
    {
        // 1) Extrai tudo que é público (headers + html)
        var surface = await _extractor.ExtractAsync(@event.Url, ct);

        // 2) Roda análises determinísticas
        var report = new ScanReport(new ScanId(@event.ScanId), @event.Url, DateTimeOffset.UtcNow);
        foreach (var analyzer in _analyzers)
        {
            foreach (var f in analyzer.Analyze(surface))
                report.AddFinding(f);
        }

        // 3) Enriquecimento por IA (opcional, só se habilitar)
        if (@event.UseAi && _ai is not null)
        {
            var enriched = await _ai.EnrichFindingsAsync(surface, report.Findings, ct);
            // evita duplicar: add só os que não existem
            foreach (var f in enriched)
            {
                if (!report.Findings.Any(x => x.Code == f.Code))
                    report.AddFinding(f);
            }
        }

        // 4) Persiste
        await _repo.SaveAsync(report, ct);

        // 5) Publica evento de finalização (pra UI reagir se quiser)
        await _bus.PublishAsync(new ScanCompleted(@event.ScanId, report), ct);
    }
}
