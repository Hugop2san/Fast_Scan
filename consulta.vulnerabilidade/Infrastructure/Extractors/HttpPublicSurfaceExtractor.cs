namespace consulta.vulnerabilidade.Infrastructure.Extractors;

using consulta.vulnerabilidade.Application.Abstractions;
using System.Net.Http.Headers;

/// <summary>
/// Extrai apenas o que é PÚBLICO do site:
/// - Status code
/// - Headers
/// - HTML (opcional, aqui a gente baixa o body também)
///
/// Importante: isso NÃO faz exploit. É coleta passiva.
/// </summary>
public sealed class HttpPublicSurfaceExtractor : IPublicSurfaceExtractor
{
    private readonly HttpClient _http;

    public HttpPublicSurfaceExtractor(HttpClient http)
    {
        _http = http;
        _http.Timeout = TimeSpan.FromSeconds(10);
    }

    public async Task<PublicSurface> ExtractAsync(string url, CancellationToken ct = default)
    {
        // 1) Normaliza URL (aqui mantemos simples; quem valida é o Domain)
        var target = url.Trim();

        // 2) Faz uma requisição GET para obter headers + html
        // Observação: muitos hosts bloqueiam HEAD (curl -I), por isso GET é mais “universal”.
        using var req = new HttpRequestMessage(HttpMethod.Get, target);

        // 3) Define um user-agent básico para evitar bloqueios toscos
        req.Headers.UserAgent.Add(new ProductInfoHeaderValue("LaunchSafeScanner", "0.1"));

        // 4) Executa request
        using var resp = await _http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);

        // 5) Coleta status code
        var status = (int)resp.StatusCode;

        // 6) Coleta headers (response + content headers)
        var headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var h in resp.Headers)
            headers[h.Key] = string.Join(", ", h.Value);

        foreach (var h in resp.Content.Headers)
            headers[h.Key] = string.Join(", ", h.Value);

        // 7) Baixa HTML (limitando tamanho pra não explodir memória em páginas grandes)
        // Nota: em MVP, limitar já é “boas práticas”.
        string? html = null;
        var contentType = resp.Content.Headers.ContentType?.MediaType ?? "";
        if (contentType.Contains("text/html", StringComparison.OrdinalIgnoreCase))
        {
            html = await ReadLimitedAsync(resp, maxChars: 120_000, ct);
        }

        // 8) Retorna a superfície pública
        return new PublicSurface(target, status, headers, html);
    }

    private static async Task<string> ReadLimitedAsync(HttpResponseMessage resp, int maxChars, CancellationToken ct)
    {
        // Lê o conteúdo como string
        var s = await resp.Content.ReadAsStringAsync(ct);

        // Limita o tamanho (evita páginas gigantes)
        if (s.Length > maxChars)
            return s[..maxChars];

        return s;
    }
}
