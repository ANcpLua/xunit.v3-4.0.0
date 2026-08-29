namespace xunitv3.template.Examples.Fixtures;

// Single test on purpose: with one test the ledger is fully deterministic at body time.
public sealed class LifecycleTests(LifecycleLedger ledger) : IClassFixture<LifecycleLedger>
{
    [Fact]
    public void Class_method_case_and_test_starts_precede_the_body() =>
        Assert.Equal(
            [LifecycleLedger.ClassStarting, LifecycleLedger.MethodStarting, LifecycleLedger.CaseStarting, LifecycleLedger.TestStarting],
            ledger.Events);
}

[Collection(LedgerCollection.Name)]
public sealed class CollectionLifecycleTests(CollectionLedger ledger)
{
    [Fact]
    public void Collection_fixture_saw_the_collection_start() => Assert.True(ledger.CollectionStarted);
}
