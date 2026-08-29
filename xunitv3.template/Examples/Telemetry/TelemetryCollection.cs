namespace xunitv3.template.Examples.Telemetry;

/// <summary>The OrderService instruments are process-wide, so every test that drives them runs serially.</summary>
[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class TelemetryCollection
{
    public const string Name = "Telemetry";
}
