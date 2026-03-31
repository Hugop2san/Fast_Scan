using consulta.vulnerabilidade.Application.AgentFix.Abstractions;
using consulta.vulnerabilidade.Domain.AgentFix;
using System.Collections.Concurrent;

namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Storage
{
    public sealed class InMemoryFixJobRepository : IFixJobRepository
    {
        private readonly ConcurrentDictionary<string, FixJob> _store = new();

        public Task AddAsync(FixJob job, CancellationToken ct = default)
        {
            _store[job.Id.ToString()] = job;
            return Task.CompletedTask;
        }

        public Task<FixJob?> GetByIdAsync(FixJobId id, CancellationToken ct = default)
        {
            _store.TryGetValue(id.ToString(), out var job);
            return Task.FromResult(job);
        }

        public Task UpdateAsync(FixJob job, CancellationToken ct = default)
        {
            _store[job.Id.ToString()] = job;
            return Task.CompletedTask;
        }
    }
}
