namespace consulta.vulnerabilidade.Application.AgentFix.Requests
{
    public sealed record RequestFixResult(
        string JobId, 
        string Status
        );
}
