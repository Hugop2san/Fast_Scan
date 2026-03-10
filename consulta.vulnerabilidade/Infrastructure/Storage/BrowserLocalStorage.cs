//using consulta.vulnerabilidade.Infrastructure.Storage;
using Microsoft.JSInterop;

namespace consulta.vulnerabilidade.Infrastructure.Storage;

public sealed class BrowserLocalStorage : ILocalStorage
{
    private readonly IJSRuntime _js;

    public BrowserLocalStorage(IJSRuntime js) => _js = js;

    public Task SetAsync(string key, string json)
        => _js.InvokeVoidAsync("localStorage.setItem", key, json).AsTask();

    public Task<string?> GetAsync(string key)
        => _js.InvokeAsync<string?>("localStorage.getItem", key).AsTask();

    public Task RemoveAsync(string key)
        => _js.InvokeVoidAsync("localStorage.removeItem", key).AsTask();
}
