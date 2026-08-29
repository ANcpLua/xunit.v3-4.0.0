using Xunit.Internal;
using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Retry;

public sealed class RetryFactDiscoverer : IXunitTestCaseDiscoverer
{
    public ValueTask<IReadOnlyCollection<IXunitTestCase>> Discover(
        ITestFrameworkDiscoveryOptions discoveryOptions,
        IXunitTestMethod testMethod,
        IFactAttribute factAttribute)
    {
        var maxRetries = (factAttribute as RetryFactAttribute)?.MaxRetries ?? RetryFactAttribute.DefaultMaxRetries;
        var details = TestIntrospectionHelper.GetTestCaseDetails(discoveryOptions, testMethod, factAttribute);

        IReadOnlyCollection<IXunitTestCase> testCases =
        [
            new RetryTestCase(
                maxRetries,
                details.ResolvedTestMethod,
                details.TestCaseDisplayName,
                details.UniqueID,
                details.Explicit,
                testLabel: null,
                factAttribute.DisableParallelization,
                details.SkipExceptions,
                details.SkipReason,
                details.SkipType,
                details.SkipUnless,
                details.SkipWhen,
                testMethod.Traits.ToReadWrite(StringComparer.OrdinalIgnoreCase),
                testMethodArguments: null,
                details.SourceFilePath,
                details.SourceLineNumber,
                details.Timeout),
        ];

        return new(testCases);
    }
}
