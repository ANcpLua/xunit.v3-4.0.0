namespace xunitv3.template.Examples.Ordering;

/// <summary>Shared gate: proves that whoever passes through it never overlaps with another holder.</summary>
public static class SerialGate
{
    private const int Delay = 20;

    private static int _holders;

    public static async Task<int> HoldAsync()
    {
        var holders = Interlocked.Increment(ref _holders);
        await Task.Delay(Delay, TestContext.Current.CancellationToken);
        Interlocked.Decrement(ref _holders);
        return holders;
    }
}

// 4.0: per-test opt-out. Under `--parallel all` these three would otherwise overlap.
public sealed class SerialFactTests
{
    [Fact(DisableParallelization = true)]
    public async Task First() => Assert.Equal(1, await SerialGate.HoldAsync());

    [Fact(DisableParallelization = true)]
    public async Task Second() => Assert.Equal(1, await SerialGate.HoldAsync());

    [Fact(DisableParallelization = true)]
    public async Task Third() => Assert.Equal(1, await SerialGate.HoldAsync());
}

// 4.0: per-collection opt-out, inherited by every class in the collection.
[CollectionDefinition(SerialCollection.Name, DisableParallelization = true)]
public sealed class SerialCollection
{
    public const string Name = "Serial collection";
}

[Collection(SerialCollection.Name)]
public sealed class SerialCollectionAlphaTests
{
    [Fact]
    public async Task Holds_alone() => Assert.Equal(1, await SerialGate.HoldAsync());
}

[Collection(SerialCollection.Name)]
public sealed class SerialCollectionBetaTests
{
    [Fact]
    public async Task Holds_alone() => Assert.Equal(1, await SerialGate.HoldAsync());
}
