using consulta.vulnerabilidade.Application.AgentFix.Abstractions;

namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Github
{
    public sealed class GitHubApiProvider : IGitProvider
    {
        public Task CreateBranchAsync(string repository, string baseBranch, string newBranch, CancellationToken ct = default)
        {
            // TODO: implementar GitHub REST API para criar refs/heads/newBranch
            throw new NotImplementedException("Implementar CreateBranchAsync.");
        }

        public Task CommitFilesAsync(string repository, string branch, string commitMessage, IReadOnlyList<FilePatch> patches, CancellationToken ct = default)
        {
            // TODO: implementar create/update file contents + commit
            throw new NotImplementedException("Implementar CommitFilesAsync.");
        }

        public Task<string> OpenPullRequestAsync(string repository, string baseBranch, string headBranch, string title, string body, CancellationToken ct = default)
        {
            // TODO: implementar POST /repos/{owner}/{repo}/pulls
            throw new NotImplementedException("Implementar OpenPullRequestAsync.");
        }
    }
}
