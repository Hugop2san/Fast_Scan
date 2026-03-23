namespace Consulta.Vulnerabilidade.Application.Scans.Handlers;

using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Domain.Scans;
using consulta.vulnerabilidade.Domain.Scans.Events;
using System.Net;

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
        var report = new ScanReport(new ScanId(@event.ScanId), @event.Url, DateTimeOffset.UtcNow);

        PublicSurface surface;
        try
        {
            // 1) Extrai tudo que e publico (headers + html)
            surface = await _extractor.ExtractAsync(@event.Url, ct);
        }
        catch (OperationCanceledException ex)
        {
            report.AddFinding(new ScanFinding(
                "SCAN_TARGET_TIMEOUT",
                "Timeout ao acessar o alvo",
                "MEDIUM",
                "O site demorou demais para responder ao scanner.",
                ex.Message
            ));

            await _repo.SaveAsync(report, ct);
            await _bus.PublishAsync(new ScanCompleted(@event.ScanId, report), ct);
            return;
        }
        catch (Exception ex)
        {
            report.AddFinding(new ScanFinding(
                "SCAN_TARGET_UNREACHABLE",
                "Nao foi possivel acessar o alvo",
                "HIGH",
                "A aplicacao nao conseguiu obter superficie publica desse endereco.",
                ex.Message
            ));

            await _repo.SaveAsync(report, ct);
            await _bus.PublishAsync(new ScanCompleted(@event.ScanId, report), ct);
            return;
        }

        if (LooksBlocked(surface))
        {
            report.AddFinding(new ScanFinding(
                "SCAN_ACCESS_DENIED_OR_LOGIN_WALL",
                "Alvo protegido por login/anti-bot",
                "LOW",
                "O site respondeu com bloqueio ou desafio de acesso. O resultado nao representa a aplicacao real por tras da protecao.",
                $"Status HTTP: {surface.StatusCode}. A coleta de HTML publico foi limitada por protecao de acesso."
            ));

            await _repo.SaveAsync(report, ct);
            await _bus.PublishAsync(new ScanCompleted(@event.ScanId, report), ct);
            return;
        }

        // 2) Roda analises deterministicas
        foreach (var analyzer in _analyzers)
        {
            foreach (var f in analyzer.Analyze(surface))
                report.AddFinding(f);
        }

        // 3) Enriquecimento por IA (opcional, so se habilitar)
        if (@event.UseAi && _ai is not null)
        {
            var enriched = await _ai.EnrichFindingsAsync(surface, report.Findings, ct);
            // evita duplicar: add so os que nao existem
            foreach (var f in enriched)
            {
                if (!report.Findings.Any(x => x.Code == f.Code))
                    report.AddFinding(f);
            }
        }

        // 4) Persiste
        await _repo.SaveAsync(report, ct);

        // 5) Publica evento de finalizacao (pra UI reagir se quiser)
        await _bus.PublishAsync(new ScanCompleted(@event.ScanId, report), ct);
    }

    private static bool LooksBlocked(PublicSurface surface)
    {
        if (surface.StatusCode is (int)HttpStatusCode.Unauthorized
            or (int)HttpStatusCode.Forbidden
            or (int)HttpStatusCode.ProxyAuthenticationRequired
            or (int)HttpStatusCode.TooManyRequests
            or (int)HttpStatusCode.ServiceUnavailable)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(surface.Html))
            return false;

        var html = surface.Html;
        return html.Contains("access denied", StringComparison.OrdinalIgnoreCase)
            || html.Contains("denied", StringComparison.OrdinalIgnoreCase)
            || html.Contains("forbidden", StringComparison.OrdinalIgnoreCase)
            || html.Contains("captcha", StringComparison.OrdinalIgnoreCase)
            || html.Contains("login", StringComparison.OrdinalIgnoreCase)
            || html.Contains("senha", StringComparison.OrdinalIgnoreCase)
            || html.Contains("cloudflare", StringComparison.OrdinalIgnoreCase)
            || html.Contains("bot", StringComparison.OrdinalIgnoreCase);
    }
}
