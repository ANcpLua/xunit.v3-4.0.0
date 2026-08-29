using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Ordering;

/// <summary>
/// Assembly-level <see cref="ITestCollectionOrderer"/>: orders collections by display name and records the result
/// so a test can prove the orderer ran. Only observable as execution order when collections run serially.
/// </summary>
public sealed class RecordingCollectionOrderer : ITestCollectionOrderer
{
    private static readonly List<string> Ordered = [];

    public static IReadOnlyList<string> OrderedDisplayNames
    {
        get
        {
            lock (Ordered)
            {
                return [.. Ordered];
            }
        }
    }

    public IReadOnlyCollection<TTestCollection> OrderTestCollections<TTestCollection>(IReadOnlyCollection<TTestCollection> testCollections)
        where TTestCollection : ITestCollection
    {
        var ordered = testCollections.OrderBy(static collection => collection.TestCollectionDisplayName, StringComparer.Ordinal).ToArray();

        lock (Ordered)
        {
            Ordered.Clear();
            Ordered.AddRange(ordered.Select(static collection => collection.TestCollectionDisplayName));
        }

        return ordered;
    }
}
