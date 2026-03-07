namespace consulta.vulnerabilidade.Domain.Scans
{

    /// <summary>
    /// Entidade raiz que representa um alvo de scan.
    /// </summary>
    public sealed class ScanTarget
    {
        public ScanId Id { get; init; }
        public string Url { get; init; }
        public DateTime CreatedAt { get; init; }
        public ScanTarget(ScanId id, string url, DateTime createdAt)
        {
            Id = id;
            Url = url;
            CreatedAt = createdAt;
        }
    }
}


