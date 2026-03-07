namespace consulta.vulnerabilidade.Infrastructure.Bus;

using consulta.vulnerabilidade.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;

public sealed class InMemoryEventBus : IEventBus
{
    private readonly IServiceProvider _sp;

    public InMemoryEventBus(IServiceProvider sp) => _sp = sp;

    public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : class
    {
        var handlers = _sp.GetServices<IEventHandler<TEvent>>();
        foreach (var h in handlers)
            await h.HandleAsync(@event, ct);
    }
}
