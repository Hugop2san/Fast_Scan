using consulta.vulnerabilidade.Application.AgentFix.Abstractions;
using consulta.vulnerabilidade.Domain.AgentFix;

namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Agent
{
    public sealed class SimpleRuleBasedFixAgent : IFixAgent
    {
        public Task<FixPlan> BuildPlanAsync(FixJob job, CancellationToken ct = default)
        {
            var branchName = $"agent/fix/{job.ScanId}/{job.FindingId}".ToLowerInvariant();

            // MVP: patch vazio para validar o pipeline fim-a-fim.
            // Proximo passo: preencher patches com mudancas reais por tipo de finding.
            var plan = new FixPlan(
                BranchName: branchName,
                CommitMessage: $"fix(security): resolve finding {job.FindingId}",
                PullRequestTitle: $"[Agent Fix] Corrige finding {job.FindingId}",
                PullRequestBody:
                    "PR criado automaticamente pelo Agent.\n\n" +
                    "- Revisao humana obrigatoria\n" +
                    "- Merge manual\n" +
                    "- Sem alteracao direta em main\n",
                Patches: new List<FilePatch>());

            return Task.FromResult(plan);
        }
    }
}
