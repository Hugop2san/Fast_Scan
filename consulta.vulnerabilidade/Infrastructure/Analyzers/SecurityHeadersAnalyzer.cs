using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Domain.Scans;

public sealed class SecurityHeadersAnalyzer : IVulnerabilityAnalyzer
{
    public IEnumerable<ScanFinding> Analyze(PublicSurface surface)
    {
        var h = surface.Headers;

        var finding = Missing(h, "strict-transport-security",
            "Falta HSTS (Strict-Transport-Security)",
            "MEDIUM",
            "Sem HSTS, navegadores podem aceitar downgrade para HTTP em alguns cenários.",
            "Adicione Strict-Transport-Security para forçar HTTPS.");
        if (finding is not null) yield return finding;

        finding = Missing(h, "x-content-type-options",
            "Falta X-Content-Type-Options",
            "LOW",
            "Sem esse header, alguns navegadores podem tentar adivinhar o tipo do arquivo.",
            "Recomendado: X-Content-Type-Options: nosniff");
        if (finding is not null) yield return finding;

        finding = Missing(h, "x-frame-options",
            "Falta X-Frame-Options",
            "MEDIUM",
            "Seu site pode ser embutido em iframes, abrindo espaço para clickjacking.",
            "Use X-Frame-Options: DENY (ou configure via CSP frame-ancestors).");
        if (finding is not null) yield return finding;

        finding = Missing(h, "content-security-policy",
            "Falta Content-Security-Policy (CSP)",
            "HIGH",
            "CSP reduz muito risco de XSS e carregamento indevido de scripts.",
            "Defina uma CSP restritiva, mesmo que inicial.");
        if (finding is not null) yield return finding;

        finding = Missing(h, "referrer-policy",
            "Falta Referrer-Policy",
            "LOW",
            "Sem isso, você pode vazar mais informações de navegação para sites externos.",
            "Ex.: Referrer-Policy: no-referrer ou strict-origin-when-cross-origin.");
        if (finding is not null) yield return finding;

        finding = Missing(h, "permissions-policy",
            "Falta Permissions-Policy",
            "LOW",
            "Sem isso, o browser pode permitir acesso a recursos como câmera ou geolocalização sem política clara.",
            "Defina Permissions-Policy conforme a necessidade.");
        if (finding is not null) yield return finding;
    }

    private static ScanFinding? Missing(
        Dictionary<string, string> headers,
        string key,
        string title,
        string severity,
        string explanation,
        string technical)
    {
        if (headers.ContainsKey(key))
            return null;

        return new ScanFinding(
            "SEC_HEADER_MISSING_" + key.ToUpperInvariant(),
            title,
            severity,
            explanation,
            technical);
    }
}
