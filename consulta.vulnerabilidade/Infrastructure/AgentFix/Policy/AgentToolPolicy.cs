using consulta.vulnerabilidade.Application.AgentFix.Abstractions;

namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Policy;

public sealed class AgentToolPolicy : IAgentToolPolicy
{
    public bool CanUseRoute(string method, string route)
        => (string.Equals(method, "POST", StringComparison.OrdinalIgnoreCase)
            && string.Equals(route, "/api/fix-jobs", StringComparison.OrdinalIgnoreCase))
           || (string.Equals(method, "GET", StringComparison.OrdinalIgnoreCase)
            && route.StartsWith("/api/fix-jobs/", StringComparison.OrdinalIgnoreCase));

    public bool CanTouchFile(string path)
    {
        var normalized = (path ?? string.Empty).Replace('\\', '/').TrimStart('/');
        return normalized.StartsWith("src/app/", StringComparison.OrdinalIgnoreCase);
    }
}
