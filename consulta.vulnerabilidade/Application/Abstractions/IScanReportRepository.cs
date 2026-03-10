using consulta.vulnerabilidade.Domain.Scans;


namespace consulta.vulnerabilidade.Application.Abstractions
{

    public interface IScanReportRepository
    {
        Task SaveAsync(ScanReport report, CancellationToken ct = default);
        Task<ScanReport?> GetLastAsync(string url, CancellationToken ct = default);
        Task<IReadOnlyList<ScanReport>> GetRecentAsync(int take = 10, CancellationToken ct = default);
        Task DeleteByUrlAsync(string url, CancellationToken ct = default);
    }
}
