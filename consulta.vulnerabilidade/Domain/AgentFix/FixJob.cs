namespace consulta.vulnerabilidade.Domain.AgentFix
{
    public class FixJob
    {
        public FixJobId Id { get; init; }
        public string ScanId { get; init; } = string.Empty;
        public string FindingId { get; init; } = string.Empty;
        public string Repository { get; init; } = string.Empty; // owner/repo
        public string BaseBranch { get; init; } = "main";
        public string? CreatedPrUrl { get; private set; }
        public FixJobStatus Status { get; private set; } = FixJobStatus.Queued;
        public string? ErrorMessage { get; private set; }
        public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? FinishedAt { get; private set; }

        public void MarkRunning() => Status = FixJobStatus.Running;

        public void MarkPendingApproval() => Status = FixJobStatus.PendingApproval;

        public void MarkPrOpened(string prUrl)
        {
            Status = FixJobStatus.PrOpened;
            CreatedPrUrl = prUrl;
            FinishedAt = DateTimeOffset.UtcNow;
        }

        public void MarkFailed(string error)
        {
            Status = FixJobStatus.Failed;
            ErrorMessage = error;
            FinishedAt = DateTimeOffset.UtcNow;
        }

    }
}
