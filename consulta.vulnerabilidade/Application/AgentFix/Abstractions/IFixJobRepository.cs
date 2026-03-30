using consulta.vulnerabilidade.Domain.AgentFix;


namespace consulta.vulnerabilidade.Application.AgentFix.Abstractions
{
    public interface IFixJobRepository
    {
        Task AddAsync(FixJob job, CancellationToken ct = default);
        Task<FixJob?> GetByIdAsync(FixJobId id, CancellationToken ct = default);
        Task UpdateAsync(FixJob job, CancellationToken ct = default);
    }
}
