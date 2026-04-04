namespace consulta.vulnerabilidade.Application.AgentFix.Abstractions;

public interface IAgentToolPolicy
{
    bool CanUseRoute(string method, string route);
    bool CanTouchFile(string path);
}
