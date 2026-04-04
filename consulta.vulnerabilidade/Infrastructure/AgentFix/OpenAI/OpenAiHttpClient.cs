using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace consulta.vulnerabilidade.Infrastructure.AgentFix.OpenAI;

public sealed class OpenAiHttpClient : IOpenAiClient
{
    private readonly HttpClient _http;
    private readonly OpenAiOptions _opt;

    public OpenAiHttpClient(HttpClient http, IOptions<OpenAiOptions> opt)
    {
        _http = http;
        _opt = opt.Value;

        _http.BaseAddress = new Uri(_opt.BaseUrl);
        _http.Timeout = TimeSpan.FromSeconds(_opt.TimeoutSeconds);
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _opt.ApiKey);
    }

    public async Task<string> CreateFixPlanJsonAsync(string prompt, CancellationToken ct = default)
    {
        var body = new
        {
            model = _opt.Model,
            input = prompt,
            text = new { format = new { type = "json_object" } }
        };

        var json = JsonSerializer.Serialize(body);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _http.PostAsync("/v1/responses", content, ct);
        var responseText = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException($"OpenAI error: {(int)response.StatusCode} - {responseText}");

        return responseText;
    }
}
