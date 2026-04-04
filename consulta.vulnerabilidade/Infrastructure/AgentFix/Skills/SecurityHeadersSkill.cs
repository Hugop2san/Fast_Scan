namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Skills;

public sealed class SecurityHeadersSkill : ISkill
{
    public bool CanHandle(string findingType)
        => findingType.Contains("header", StringComparison.OrdinalIgnoreCase);

    public string BuildPrompt(string findingType, string evidence)
        => $"""
        Contexto: finding de headers de seguranca.
        FindingType: {findingType}
        Evidence: {evidence}
        Gere patches minimos para aplicar headers seguros sem quebrar comportamento atual.
        """;
}
