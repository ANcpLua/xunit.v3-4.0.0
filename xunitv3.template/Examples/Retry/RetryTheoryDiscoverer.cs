using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Retry;

public sealed class RetryTheoryDiscoverer : TheoryDiscoverer
{
    // Pre-enumerated, serializable data: one RetryTestCase per row. This is where the 4.0
    // row label (ITheoryDataRow.Label -> TestLabel) and per-row parallelization opt-out are carried over.
    protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForDataRow(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod,
        ITheoryAttribute theoryAttribute,
        ITheoryDataRow dataRow,
        object?[] testMethodArguments,
        string? index)
    {
        var maxRetries = MaxRetries(theoryAttribute);
        var details = TestIntrospectionHelper.GetTestCaseDetailsForTheoryDataRow(discoveryOptions, testMethod, theoryAttribute, dataRow, testMethodArguments, index);

        IReadOnlyCollection<IXunitTestCase> testCases =
        [
            new RetryTestCase(
                maxRetries,
                details.ResolvedTestMethod,
                details.TestCaseDisplayName,
                details.UniqueID,
                details.Explicit,
                dataRow.Label,
                dataRow.DisableParallelization ?? theoryAttribute.DisableParallelization,
                details.SkipExceptions,
                details.SkipReason,
                details.SkipType,
                details.SkipUnless,
                details.SkipWhen,
                TestIntrospectionHelper.GetTraits(testMethod, dataRow),
                testMethodArguments,
                details.SourceFilePath,
                details.SourceLineNumber,
                details.Timeout),
        ];

        return new(testCases);
    }

    // Pre-enumeration disabled (or data not serializable): a single test case that enumerates
    // its rows at execution time. An unconditional skip needs no retry machinery at all.
    protected override ValueTask<IReadOnlyCollection<IXunitTestCase>> CreateTestCasesForTheory(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod,
        ITheoryAttribute theoryAttribute)
    {
        var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, theoryAttribute);
        var traits = testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase);

        IXunitTestCase testCase =
            details is { SkipReason: not null, SkipUnless: null, SkipWhen: null }
                ? new XunitTestCase(
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    details.SkipExceptions,
                    details.SkipReason,
                    details.SkipType,
                    details.SkipUnless,
                    details.SkipWhen,
                    traits,
                    sourceFilePath: details.SourceFilePath,
                    sourceLineNumber: details.SourceLineNumber,
                    timeout: details.Timeout)
                : new RetryDelayEnumeratedTestCase(
                    MaxRetries(theoryAttribute),
                    details.ResolvedTestMethod,
                    details.TestCaseDisplayName,
                    details.UniqueID,
                    details.Explicit,
                    theoryAttribute.SkipTestWithoutData,
                    details.SkipExceptions,
                    details.SkipReason,
                    details.SkipType,
                    details.SkipUnless,
                    details.SkipWhen,
                    traits,
                    details.SourceFilePath,
                    details.SourceLineNumber,
                    details.Timeout);

        return new([testCase]);
    }

    private static int MaxRetries(ITheoryAttribute theoryAttribute) =>
        (theoryAttribute as RetryTheoryAttribute)?.MaxRetries ?? RetryFactAttribute.DefaultMaxRetries;
}
