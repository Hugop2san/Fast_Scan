namespace consulta.vulnerabilidade.Infrastructure.AgentFix.OpenAI;

public interface IOpenAiClient
{
    Task<string> CreateFixPlanJsonAsync(string prompt, CancellationToken ct = default);
}
