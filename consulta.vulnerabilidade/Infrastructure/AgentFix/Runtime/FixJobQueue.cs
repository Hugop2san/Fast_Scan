using System.Threading.Channels;
using consulta.vulnerabilidade.Domain.AgentFix;

namespace consulta.vulnerabilidade.Infrastructure.AgentFix.Runtime;

public sealed class FixJobQueue
{
    private readonly Channel<FixJobId> _channel = Channel.CreateUnbounded<FixJobId>();

    public ValueTask EnqueueAsync(FixJobId id, CancellationToken ct = default)
        => _channel.Writer.WriteAsync(id, ct);

    public ValueTask<FixJobId> DequeueAsync(CancellationToken ct = default)
        => _channel.Reader.ReadAsync(ct);
}
