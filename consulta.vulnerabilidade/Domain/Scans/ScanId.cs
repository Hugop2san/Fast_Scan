namespace consulta.vulnerabilidade.Domain.Scans
{
    /// <summary>
    /// Value Object simples para identificar um scan.
    /// </summary>
    public readonly record struct ScanId(Guid Value)
    {
        public static ScanId New() => new(Guid.NewGuid());
    }
}


