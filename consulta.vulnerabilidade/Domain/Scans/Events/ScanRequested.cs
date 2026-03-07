namespace consulta.vulnerabilidade.Domain.Scans.Events
{
 

    public sealed record ScanRequested(Guid ScanId, string Url, bool UseAi); 
}
