namespace Consulta.Vulnerabilidade.Infrastructure.Storage;

using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Domain.Scans;
using consulta.vulnerabilidade.Infrastructure.Storage;
using System.Text.Json;

public sealed class ScanReportRepositoryLocalStorage : IScanReportRepository
{
    private const string KEY_RECENT = "launchsafe:recent";
    private readonly ILocalStorage _ls;

    public ScanReportRepositoryLocalStorage(ILocalStorage ls) => _ls = ls;

    public async Task SaveAsync(ScanReport report, CancellationToken ct = default)
    {
        var recent = (await GetRecentInternalAsync())?.ToList() ?? new List<ScanReport>();

        // Remove duplicados por URL e mantém o mais recente
        recent.RemoveAll(r => r.Url.Equals(report.Url, StringComparison.OrdinalIgnoreCase));
        recent.Insert(0, report);

        // Limita histórico
        recent = recent.Take(20).ToList();

        await _ls.SetAsync(KEY_RECENT, JsonSerializer.Serialize(recent));
        await _ls.SetAsync(KeyLastByUrl(report.Url), JsonSerializer.Serialize(report));
    }

    public async Task<ScanReport?> GetLastAsync(string url, CancellationToken ct = default)
    {
        var json = await _ls.GetAsync(KeyLastByUrl(url));
        return json is null ? null : JsonSerializer.Deserialize<ScanReport>(json);
    }

    public async Task<IReadOnlyList<ScanReport>> GetRecentAsync(int take = 10, CancellationToken ct = default)
    {
        var recent = await GetRecentInternalAsync() ?? new List<ScanReport>();
        return recent.Take(take).ToList();
    }

    private async Task<List<ScanReport>?> GetRecentInternalAsync()
    {
        var json = await _ls.GetAsync(KEY_RECENT);
        return json is null ? null : JsonSerializer.Deserialize<List<ScanReport>>(json);
    }

    private static string KeyLastByUrl(string url) => "launchsafe:last:" + url.ToLowerInvariant();
}
