namespace xunitv3.template.Examples.Ordering;

// [TestClass(DisableParallelization = true)] is the 4.0 per-class opt-out: the ordering
// assertions below stay valid even if xunit.runner.json switches "parallelMode" to "all".
[TestClass(DisableParallelization = true)]
[TestMethodOrderer(typeof(PriorityOrderer))]
public sealed class PriorityOrderingTests
{
    private const int First = -1;
    private const int Last = 1;

    private static readonly List<string> Executed = [];

    [Fact, TestPriority(First)]
    public void RunsFirst()
    {
        Executed.Add(nameof(RunsFirst));
        Assert.Equal([nameof(RunsFirst)], Executed);
    }

    [Fact]
    public void RunsSecond()
    {
        Executed.Add(nameof(RunsSecond));
        Assert.Equal([nameof(RunsFirst), nameof(RunsSecond)], Executed);
    }

    [Fact, TestPriority(Last)]
    public void RunsLast()
    {
        Executed.Add(nameof(RunsLast));
        Assert.Equal([nameof(RunsFirst), nameof(RunsSecond), nameof(RunsLast)], Executed);
    }
}

[TestClass(DisableParallelization = true)]
public sealed class TestCaseOrderingTests
{
    private static readonly List<int> Executed = [];

    [Theory, TestCaseOrderer(typeof(DescendingDisplayNameOrderer))]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void RowsRunInDescendingOrder(int row)
    {
        Executed.Add(row);
        Assert.Equal(Executed.OrderDescending(), Executed);
    }
}
