namespace consulta.vulnerabilidade.Application.Abstractions
{
    public interface IEventHandler<in TEvent> where TEvent : class
    {
        Task HandleAsync(TEvent @event, CancellationToken ct = default);
    }
}


