using System.Text.Json;
using consulta.vulnerabilidade.Application.AgentFix.Abstractions;
using consulta.vulnerabilidade.Domain.AgentFix;
using consulta.vulnerabilidade.Infrastructure.AgentFix.OpenAI;
using consulta.vulnerabilidade.Infrastructure.AgentFix.Skills;

namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Agent;

public sealed class OpenAiFixAgent : IFixAgent
{
    private readonly IOpenAiClient _client;
    private readonly IEnumerable<ISkill> _skills;

    public OpenAiFixAgent(IOpenAiClient client, IEnumerable<ISkill> skills)
    {
        _client = client;
        _skills = skills;
    }

    public async Task<FixPlan> BuildPlanAsync(FixJob job, CancellationToken ct = default)
    {
        var skill = _skills.FirstOrDefault(s => s.CanHandle(job.FindingId));
        var skillInstructions = skill?.BuildPrompt(job.FindingId, $"scan={job.ScanId}") ?? "Sem skill especifica; gerar plano generico.";

        var prompt = $$"""
        Voce deve responder SOMENTE JSON valido.
        Gere plano de correcao para finding {job.FindingId} do scan {job.ScanId}.
        Regras obrigatorias:
        - branchName deve seguir agent/fix/<scanId>/<findingId>
        - nunca alterar pipeline de deploy
        - nunca criar merge automatico
        - manter mudancas minimas e revisaveis
        Skill ativa:
        {skillInstructions}
        JSON esperado:
        {
          "branchName":"...",
          "commitMessage":"...",
          "pullRequestTitle":"...",
          "pullRequestBody":"...",
          "patches":[{"path":"...","newContent":"..."}]
        }
        """;

        var raw = await _client.CreateFixPlanJsonAsync(prompt, ct);
        var planJson = ExtractOutputText(raw);
        return ParsePlan(planJson, job);
    }

    private static string ExtractOutputText(string raw)
    {
        using var doc = JsonDocument.Parse(raw);
        var root = doc.RootElement;

        if (root.TryGetProperty("output_text", out var directText) && directText.ValueKind == JsonValueKind.String)
            return directText.GetString() ?? "{}";

        if (root.TryGetProperty("output", out var output) && output.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in output.EnumerateArray())
            {
                if (!item.TryGetProperty("content", out var content) || content.ValueKind != JsonValueKind.Array)
                    continue;

                foreach (var block in content.EnumerateArray())
                {
                    if (block.TryGetProperty("type", out var type)
                        && type.ValueKind == JsonValueKind.String
                        && string.Equals(type.GetString(), "output_text", StringComparison.OrdinalIgnoreCase)
                        && block.TryGetProperty("text", out var text)
                        && text.ValueKind == JsonValueKind.String)
                    {
                        return text.GetString() ?? "{}";
                    }
                }
            }
        }

        return "{}";
    }

    private static FixPlan ParsePlan(string text, FixJob job)
    {
        using var planDoc = JsonDocument.Parse(string.IsNullOrWhiteSpace(text) ? "{}" : text);
        var root = planDoc.RootElement;

        var branchName = GetString(root, "branchName") ?? $"agent/fix/{job.ScanId}/{job.FindingId}";
        var commitMessage = GetString(root, "commitMessage") ?? $"fix(security): {job.FindingId}";
        var prTitle = GetString(root, "pullRequestTitle") ?? $"[Agent Fix] {job.FindingId}";
        var prBody = GetString(root, "pullRequestBody") ?? "PR gerado pelo agent";

        var patches = new List<FilePatch>();
        if (root.TryGetProperty("patches", out var patchesEl) && patchesEl.ValueKind == JsonValueKind.Array)
        {
            foreach (var patch in patchesEl.EnumerateArray())
            {
                var path = GetString(patch, "path");
                var content = GetString(patch, "newContent");
                if (string.IsNullOrWhiteSpace(path) || content is null)
                    continue;
                patches.Add(new FilePatch(path, content));
            }
        }

        return new FixPlan(branchName, commitMessage, prTitle, prBody, patches);
    }

    private static string? GetString(JsonElement element, string name)
        => element.TryGetProperty(name, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;
}
