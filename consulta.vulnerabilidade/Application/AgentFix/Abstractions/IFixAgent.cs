using consulta.vulnerabilidade.Domain.AgentFix;

namespace consulta.vulnerabilidade.Application.AgentFix.Abstractions
{
    public interface IFixAgent
    {
        Task<FixPlan> BuildPlanAsync(FixJob job, CancellationToken ct = default);
    }

    public sealed record FixPlan(
        string BranchName,
        string CommitMessage,
        string PullRequestTitle,
        string PullRequestBody,
        IReadOnlyList<FilePatch> Patches);

    public sealed record FilePatch(string Path, string NewContent);
}
