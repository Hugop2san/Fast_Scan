
namespace consulta.vulnerabilidade.Infrastructure.Storage;

public interface ILocalStorage
{
    Task SetAsync(string key, string json);
    Task<string?> GetAsync(string key);
}


