using System.Runtime.CompilerServices;
using Xunit.v3;

namespace xunitv3.template.Examples.Retry;

/// <summary>Works like <c>[Fact]</c>, but a failing test is re-run up to <see cref="MaxRetries"/> times before the failure is reported.</summary>
[XunitTestCaseDiscoverer(typeof(RetryFactDiscoverer))]
public sealed class RetryFactAttribute(
    [CallerFilePath] string? sourceFilePath = null,
    [CallerLineNumber] int sourceLineNumber = -1)
    : FactAttribute(sourceFilePath, sourceLineNumber)
{
    public const int DefaultMaxRetries = 3;

    public int MaxRetries { get; set; } = DefaultMaxRetries;
}
