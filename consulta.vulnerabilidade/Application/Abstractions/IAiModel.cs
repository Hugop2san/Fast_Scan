namespace consulta.vulnerabilidade.Application.Abstractions
{
    using consulta.vulnerabilidade.Domain.Scans;
    using static consulta.vulnerabilidade.Application.Abstractions.IPublicSurfaceExtractor;

    /// <summary>
    /// Abstração para plugar qualquer modelo (OpenAI, local, etc).
    /// Não faz parte do MVP “hard”, mas deixa o slot pronto no DI.
    /// </summary>
    public interface IAiModel
    {
        Task<IReadOnlyList<ScanFinding>> EnrichFindingsAsync(
            PublicSurface surface,
            IReadOnlyList<ScanFinding> current,
            CancellationToken ct = default);
    }
}
