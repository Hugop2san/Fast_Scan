using consulta.vulnerabilidade.Domain.Scans;

namespace consulta.vulnerabilidade.Domain.Scans;


    /// <summary>
    /// Agregado raiz: o resultado final de um scan.
    /// </summary>
public sealed class ScanReport
{
    public ScanId Id { get; }
    public string Url { get; }
    public DateTimeOffset CreatedAt { get; }
    public int Score { get; private set; } // 0..100
    public IReadOnlyList<ScanFinding> Findings => _findings;

    private readonly List<ScanFinding> _findings = new();

    public ScanReport(ScanId id, string url, DateTimeOffset createdAt)
    {
        Id = id;
        Url = url;
        CreatedAt = createdAt;
        Score = 100;
    }

    public void AddFinding(ScanFinding finding)
    {
        _findings.Add(finding);
        Score = Math.Clamp(Score - SeverityPenalty(finding.Severity), 0, 100);
    }

    private static int SeverityPenalty(string severity) => severity switch
    {
        "HIGH" => 25,
        "MEDIUM" => 12,
        "LOW" => 5,
        _ => 8
    };
}


