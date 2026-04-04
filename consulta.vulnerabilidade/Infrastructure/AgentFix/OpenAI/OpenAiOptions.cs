namespace consulta.vulnerabilidade.Infrastructure.AgentFix.OpenAI;

public sealed class OpenAiOptions
{
    public const string SectionName = "OpenAI";

    public string BaseUrl { get; set; } = "https://api.openai.com";
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = "gpt-4.1-mini";
    public int TimeoutSeconds { get; set; } = 60;
}
