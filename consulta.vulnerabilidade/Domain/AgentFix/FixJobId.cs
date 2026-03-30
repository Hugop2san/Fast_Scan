namespace consulta.vulnerabilidade.Domain.AgentFix
{
    public readonly record struct FixJobId(Guid Value)
    {
        public static FixJobId New() => new(Guid.NewGuid());
        public override string ToString() => Value.ToString("N");
    }
}

