namespace consulta.vulnerabilidade.Infrastructure.Analyzers;

using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Domain.Scans;
public sealed class BlazorHintsAnalyzer : IVulnerabilityAnalyzer
{
    public IEnumerable<ScanFinding> Analyze(PublicSurface surface)
    {
        if (surface.Html is null) yield break;

        if (surface.Html.Contains("blazor.server", StringComparison.OrdinalIgnoreCase) ||
            surface.Html.Contains("Blazor:{\"type\":\"server\"", StringComparison.OrdinalIgnoreCase))
        {
            yield return new ScanFinding(
                "INFO_BLAZOR_SERVER",
                "Aplicação aparenta usar Blazor Server",
                "LOW",
                "Blazor Server depende de conexão persistente. Recomenda-se rate limiting e hardening.",
                "Considere rate limiting + headers + políticas de sessão."
            );
        }
    }
}
