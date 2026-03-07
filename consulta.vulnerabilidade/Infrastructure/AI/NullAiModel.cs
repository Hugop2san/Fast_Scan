namespace consulta.vulnerabilidade.Infrastructure.AI;

using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Domain.Scans;

public sealed class NullAiModel : IAiModel
{
    public Task<IReadOnlyList<ScanFinding>> EnrichFindingsAsync(
        PublicSurface surface,
        IReadOnlyList<ScanFinding> current,
        CancellationToken ct = default)
    {
        // MVP: não faz nada. Mantém o contrato pronto para plugar OpenAI depois.
        return Task.FromResult<IReadOnlyList<ScanFinding>>(Array.Empty<ScanFinding>());
    }
}
