namespace consulta.vulnerabilidade.Infrastructure.Analyzers;

using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Domain.Scans;

public sealed class FingerprintAnalyzer : IVulnerabilityAnalyzer
{
    public IEnumerable<ScanFinding> Analyze(PublicSurface surface)
    {
        var h = surface.Headers;

        if (h.TryGetValue("server", out var server))
        {
            yield return new ScanFinding(
                "INFO_SERVER_HEADER",
                "Header 'Server' expõe tecnologia",
                "LOW",
                "Isso pode revelar detalhes do stack e facilitar ataques automatizados.",
                $"Server: {server}"
            );
        }

        if (h.Keys.Any(k => k.Contains("x-powered-by", StringComparison.OrdinalIgnoreCase)))
        {
            yield return new ScanFinding(
                "INFO_X_POWERED_BY",
                "Header 'X-Powered-By' expõe tecnologia",
                "LOW",
                "Expor tecnologia não é uma falha crítica, mas é uma boa prática reduzir fingerprint.",
                "Remova/oculte X-Powered-By quando possível."
            );
        }
    }
}
