using System.Collections.Concurrent;
using Xunit.v3;
using xunitv3.template.Examples.Fixtures;

[assembly: AssemblyFixture(typeof(TelemetryFixture))]

namespace xunitv3.template.Examples.Fixtures;

/// <summary>
/// Assembly fixture (one instance per test assembly, injected by constructor) that also opts into the 4.0
/// fixture lifecycle hooks: <see cref="INotifyTestLifecycle"/> is called around every test in the assembly.
/// </summary>
public sealed class TelemetryFixture : IAsyncLifetime, INotifyTestLifecycle
{
    private static int _instances;

    private readonly ConcurrentDictionary<string, byte> _started = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, byte> _finished = new(StringComparer.Ordinal);

    public TelemetryFixture() => Interlocked.Increment(ref _instances);

    public static int Instances => _instances;

    public bool Initialized { get; private set; }

    public IReadOnlyCollection<string> Started => _started.Keys.ToArray();

    public IReadOnlyCollection<string> Finished => _finished.Keys.ToArray();

    public ValueTask InitializeAsync()
    {
        Initialized = true;
        return default;
    }

    public ValueTask DisposeAsync() => default;

    public void OnTestStarting(IXunitTest test) => _started[test.TestDisplayName] = default;

    public void OnTestFinished(IXunitTest test) => _finished[test.TestDisplayName] = default;
}
