using System.Collections.Concurrent;

namespace xunitv3.template.Examples.Retry;

/// <summary>Class fixture: one instance per test class, so it survives the re-instantiation that happens on every attempt.</summary>
public sealed class AttemptCounter
{
    private readonly ConcurrentDictionary<string, int> _attempts = new(StringComparer.Ordinal);

    public int Next(string key) => _attempts.AddOrUpdate(key, 1, static (_, attempts) => attempts + 1);
}

public sealed class RetryTests(AttemptCounter counter) : IClassFixture<AttemptCounter>
{
    private const int MaxRetries = 3;

    private int Attempt => counter.Next(TestContext.Current.Test!.TestDisplayName);

    [RetryFact(MaxRetries = MaxRetries)]
    public void PassesOnSecondAttempt() => Assert.Equal(2, Attempt);

    [RetryTheory(MaxRetries = MaxRetries)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void PassesOnNthAttempt(int passOnAttempt) => Assert.Equal(passOnAttempt, Attempt);

    [RetryTheory(MaxRetries = MaxRetries, DisableDiscoveryEnumeration = true)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void PassesOnNthAttemptWhenEnumeratedAtRunTime(int passOnAttempt) => Assert.Equal(passOnAttempt, Attempt);
}
