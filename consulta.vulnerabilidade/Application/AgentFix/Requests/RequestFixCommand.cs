namespace consulta.vulnerabilidade.Application.AgentFix.Requests
{
public sealed record RequestFixCommand(
string ScanId,
string FindingId,
string Repository,
string BaseBranch,
string Severity = "medium");
}
