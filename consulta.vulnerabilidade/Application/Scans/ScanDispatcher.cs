using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Domain.Scans.Events;
using consulta.vulnerabilidade.Application.Abstractions;
using consulta.vulnerabilidade.Domain.Scans;
using consulta.vulnerabilidade.Domain.Scans.Events;



namespace consulta.vulnerabilidade.Application.Scans
{

    public sealed class ScanDispatcher
    {
        private readonly IEventBus _bus;

        public ScanDispatcher(IEventBus bus) => _bus = bus;

        public async Task<Guid> RequestScanAsync(string url, bool useAi, CancellationToken ct = default)
        {
            var id = Guid.NewGuid();
            await _bus.PublishAsync(new ScanRequested(id, url, useAi), ct);
            return id;
        }
    }
}
