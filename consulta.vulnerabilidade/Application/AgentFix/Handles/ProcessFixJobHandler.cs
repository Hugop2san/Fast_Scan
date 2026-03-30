using consulta.vulnerabilidade.Application.AgentFix.Abstractions;
using consulta.vulnerabilidade.Domain.AgentFix;

namespace consulta.vulnerabilidade.Application.AgentFix.Handles
{
    public sealed class ProcessFixJobHandler
    {
        private readonly IFixJobRepository _repo;
        private readonly IFixAgent _agent;
        private readonly IGitProvider _git;

        public ProcessFixJobHandler(
            IFixJobRepository repo,
            IFixAgent agent,
            IGitProvider git)
        {
            _repo = repo;
            _agent = agent;
            _git = git;
        }

        public async Task HandleAsync(string jobIdRaw, CancellationToken ct = default)
        {
            if (!Guid.TryParse(jobIdRaw, out var parsed))
                throw new InvalidOperationException("JobId invalido.");

            var jobId = new FixJobId(parsed);
            var job = await _repo.GetByIdAsync(jobId, ct) ?? throw new InvalidOperationException("FixJob nao encontrado.");

            try
            {
                job.MarkRunning();
                await _repo.UpdateAsync(job, ct);

                var plan = await _agent.BuildPlanAsync(job, ct);

                await _git.CreateBranchAsync(job.Repository, job.BaseBranch, plan.BranchName, ct);
                await _git.CommitFilesAsync(job.Repository, plan.BranchName, plan.CommitMessage, plan.Patches, ct);
                var prUrl = await _git.OpenPullRequestAsync(job.Repository, job.BaseBranch, plan.BranchName, plan.PullRequestTitle, plan.PullRequestBody, ct);

                job.MarkPrOpened(prUrl);
                await _repo.UpdateAsync(job, ct);
            }
            catch (Exception ex)
            {
                job.MarkFailed(ex.Message);
                await _repo.UpdateAsync(job, ct);
                throw;
            }
        }
    }
}
