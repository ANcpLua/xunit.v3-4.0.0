namespace xunitv3.template.Examples.Ordering;

public sealed class CollectionOrdererTests
{
    [Fact]
    public void Assembly_orderer_ran_over_this_collection()
    {
        var names = RecordingCollectionOrderer.OrderedDisplayNames;

        Assert.Contains(TestContext.Current.TestCollection!.TestCollectionDisplayName, names);
        Assert.Equal(names.Order(StringComparer.Ordinal), names);
    }
}

// DisableParallelization keeps the two classes serial even under `--parallel all`; the orderer then decides the order.
[CollectionDefinition(OrderedClasses.Name, DisableParallelization = true)]
[TestClassOrderer(typeof(AlphabeticalClassOrderer))]
public sealed class OrderedClasses
{
    public const string Name = "Ordered classes";

    private static readonly List<string> Executed = [];

    public static IReadOnlyList<string> Record(string testClass)
    {
        lock (Executed)
        {
            Executed.Add(testClass);
            return [.. Executed];
        }
    }
}

[Collection(OrderedClasses.Name)]
public sealed class AlphaClassTests
{
    [Fact]
    public void Runs_before_beta() => Assert.Equal([nameof(AlphaClassTests)], OrderedClasses.Record(nameof(AlphaClassTests)));
}

[Collection(OrderedClasses.Name)]
public sealed class BetaClassTests
{
    [Fact]
    public void Runs_after_alpha() => Assert.Equal([nameof(AlphaClassTests), nameof(BetaClassTests)], OrderedClasses.Record(nameof(BetaClassTests)));
}
