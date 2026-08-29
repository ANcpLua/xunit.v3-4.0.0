using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Ordering;

/// <summary>
/// xUnit.net v3 4.0 <see cref="ITestCaseOrderer"/>: orders the test cases of a single method
/// (i.e. the pre-enumerated rows of a theory) by display name, descending.
/// Apply with <c>[TestCaseOrderer(typeof(DescendingDisplayNameOrderer))]</c> on a method, class, collection definition, or the assembly.
/// </summary>
public sealed class DescendingDisplayNameOrderer : ITestCaseOrderer
{
    public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases)
        where TTestCase : notnull, ITestCase =>
        [.. testCases.OrderByDescending(static testCase => testCase.TestCaseDisplayName, StringComparer.Ordinal)];
}
