using System.Collections.Concurrent;
using Xunit.v3;

namespace xunitv3.template.Examples.Fixtures;

/// <summary>
/// Class fixture opting into the 4.0 lifecycle notifications below the assembly level, mixing the sync and
/// async variants to show both shapes.
/// </summary>
public sealed class LifecycleLedger :
    INotifyTestClassLifecycleAsync,
    INotifyTestMethodLifecycle,
    INotifyTestCaseLifecycleAsync,
    INotifyTestLifecycleAsync
{
    public const string ClassStarting = nameof(ClassStarting);
    public const string ClassFinished = nameof(ClassFinished);
    public const string MethodStarting = nameof(MethodStarting);
    public const string MethodFinished = nameof(MethodFinished);
    public const string CaseStarting = nameof(CaseStarting);
    public const string CaseFinished = nameof(CaseFinished);
    public const string TestStarting = nameof(TestStarting);
    public const string TestFinished = nameof(TestFinished);

    private readonly ConcurrentQueue<string> _events = new();

    public IReadOnlyList<string> Events => [.. _events];

    public ValueTask OnTestClassStartingAsync(IXunitTestClass testClass) => Record(ClassStarting);

    public ValueTask OnTestClassFinishedAsync(IXunitTestClass testClass) => Record(ClassFinished);

    public void OnTestMethodStarting(IXunitTestMethod testMethod) => _events.Enqueue(MethodStarting);

    public void OnTestMethodFinished(IXunitTestMethod testMethod) => _events.Enqueue(MethodFinished);

    public ValueTask OnTestCaseStartingAsync(IXunitTestCase testCase) => Record(CaseStarting);

    public ValueTask OnTestCaseFinishedAsync(IXunitTestCase testCase) => Record(CaseFinished);

    public ValueTask OnTestStartingAsync(IXunitTest test) => Record(TestStarting);

    public ValueTask OnTestFinishedAsync(IXunitTest test) => Record(TestFinished);

    private ValueTask Record(string @event)
    {
        _events.Enqueue(@event);
        return default;
    }
}

/// <summary>Collection fixture receiving the collection-level notification.</summary>
public sealed class CollectionLedger : INotifyTestCollectionLifecycle
{
    public bool CollectionStarted { get; private set; }

    public void OnTestCollectionStarting(IXunitTestCollection testCollection) => CollectionStarted = true;

    public void OnTestCollectionFinished(IXunitTestCollection testCollection)
    {
    }
}

[CollectionDefinition(LedgerCollection.Name)]
public sealed class LedgerCollection : ICollectionFixture<CollectionLedger>
{
    public const string Name = "Ledger collection";
}
