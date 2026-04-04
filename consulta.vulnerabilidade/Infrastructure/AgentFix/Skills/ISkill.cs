namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Skills;

public interface ISkill
{
    bool CanHandle(string findingType);
    string BuildPrompt(string findingType, string evidence);
}
