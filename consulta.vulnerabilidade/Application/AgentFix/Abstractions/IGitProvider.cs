namespace consulta.vulnerabilidade.Application.AgentFix.Abstractions
{
    public interface IGitProvider
    {
        Task CreateBranchAsync(string repository, string baseBranch, string newBranch, CancellationToken ct = default);
        Task CommitFilesAsync(string repository, string branch, string commitMessage, IReadOnlyList<FilePatch> patches, CancellationToken ct = default);
        Task<string> OpenPullRequestAsync(string repository, string baseBranch, string headBranch, string title, string body, CancellationToken ct = default);
    }
}
