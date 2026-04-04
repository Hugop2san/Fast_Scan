namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Skills;

public sealed class DependencyUpgradeSkill : ISkill
{
    public bool CanHandle(string findingType)
        => findingType.Contains("dependency", StringComparison.OrdinalIgnoreCase)
           || findingType.Contains("package", StringComparison.OrdinalIgnoreCase);

    public string BuildPrompt(string findingType, string evidence)
        => $"""
        Contexto: finding de dependencia desatualizada.
        FindingType: {findingType}
        Evidence: {evidence}
        Gere patches minimos para atualizar versao mantendo compatibilidade do projeto.
        """;
}
