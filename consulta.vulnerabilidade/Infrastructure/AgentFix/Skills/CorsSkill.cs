namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Skills;

public sealed class CorsSkill : ISkill
{
    public bool CanHandle(string findingType)
        => findingType.Contains("cors", StringComparison.OrdinalIgnoreCase);

    public string BuildPrompt(string findingType, string evidence)
        => $"""
        Contexto: finding de CORS.
        FindingType: {findingType}
        Evidence: {evidence}
        Gere patches minimos para restringir origem/metodos/headers indevidos.
        """;
}
