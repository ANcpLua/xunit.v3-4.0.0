using System.Runtime.CompilerServices;
using Xunit.v3;

namespace xunitv3.template.Examples.Retry;

/// <summary>Works like <c>[Theory]</c>, but each failing row is re-run up to <see cref="MaxRetries"/> times before the failure is reported.</summary>
[XunitTestCaseDiscoverer(typeof(RetryTheoryDiscoverer))]
public sealed class RetryTheoryAttribute(
    [CallerFilePath] string? sourceFilePath = null,
    [CallerLineNumber] int sourceLineNumber = -1)
    : TheoryAttribute(sourceFilePath, sourceLineNumber)
{
    public int MaxRetries { get; set; } = RetryFactAttribute.DefaultMaxRetries;
}
