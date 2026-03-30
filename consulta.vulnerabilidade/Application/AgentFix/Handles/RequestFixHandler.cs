using consulta.vulnerabilidade.Application.AgentFix.Abstractions;
using consulta.vulnerabilidade.Application.AgentFix.Requests;
using consulta.vulnerabilidade.Domain.AgentFix;

namespace consulta.vulnerabilidade.Application.AgentFix.Handlers
{
    public sealed class RequestFixHandler
    {
        private readonly IFixJobRepository _repo;

        public RequestFixHandler(IFixJobRepository repo)
        {
            _repo = repo;
        }

        public async Task<RequestFixResult> HandleAsync(RequestFixCommand cmd, CancellationToken ct = default)
        {
            var baseBranch = string.IsNullOrWhiteSpace(cmd.BaseBranch) ? "main" : cmd.BaseBranch;

            var job = new FixJob
            {
                Id = FixJobId.New(),
                ScanId = cmd.ScanId,
                FindingId = cmd.FindingId,
                Repository = cmd.Repository,
                BaseBranch = baseBranch
            };

            await _repo.AddAsync(job, ct);
            return new RequestFixResult(job.Id.ToString(), FixJobStatus.Queued.ToString());
        }
    }
}