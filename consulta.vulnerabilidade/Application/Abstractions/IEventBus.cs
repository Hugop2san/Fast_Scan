namespace consulta.vulnerabilidade.Application.Abstractions
{
    public interface IEventBus
    {
        Task PublishAsync<TEvent>(TEvent @event, CancellationToken ct = default) where TEvent : class;
    }

}





