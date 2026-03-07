namespace consulta.vulnerabilidade.Application.Abstractions
{


    public sealed record PublicSurface(
        string Url,
        int StatusCode,
        Dictionary<string, string> Headers,
        string? Html // pode ser null se não buscar corpo
    );
        
    public interface IPublicSurfaceExtractor
    {
        Task<PublicSurface> ExtractAsync(string url, CancellationToken ct = default);
    }

    
}
