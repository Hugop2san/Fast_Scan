namespace consulta.vulnerabilidade.Domain.AgentFix
{


    public enum FixJobStatus
    {
        Queued = 0,
        PendingApproval = 1,
        Running = 2,
        PrOpened = 3,
        Failed = 4,
        Cancelled = 5
    }
    
}
