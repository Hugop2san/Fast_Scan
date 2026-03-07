namespace consulta.vulnerabilidade.Domain.Scans
{
    /// <summary>
    /// Achado encontrado na análise (o que o usuário vai ver).
    /// </summary>
    public sealed record ScanFinding(
        string Code,            // ex: "SEC_HEADER_MISSING_CSP"
        string Title,           // ex: "Falta Content-Security-Policy"
        string Severity,        // "LOW" | "MEDIUM" | "HIGH"
        string Explanation,     // explicação leiga
        string TechnicalDetails // detalhes técnicos (para a opção C)
    );
}
