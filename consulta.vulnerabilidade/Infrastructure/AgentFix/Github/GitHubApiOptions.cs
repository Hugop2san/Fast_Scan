namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Github
{
    public sealed class GitHubApiOptions
    {
        public const string SectionName = "GitHub";

        public string BaseUrl { get; set; } = "https://api.github.com";
        public string Token { get; set; } = string.Empty;
        public string BotName { get; set; } = "agent-fix-bot";
        public string BotEmail { get; set; } = "agent-fix-bot@example.local";
    }
}
