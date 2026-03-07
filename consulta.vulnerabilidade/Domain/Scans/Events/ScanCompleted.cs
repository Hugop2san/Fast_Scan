using consulta.vulnerabilidade.Domain.Scans;


namespace consulta.vulnerabilidade.Domain.Scans.Events
{

    public sealed record ScanCompleted(Guid ScanId, ScanReport Report);
}
