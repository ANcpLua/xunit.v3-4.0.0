using System.ComponentModel;
using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Retry;

/// <summary>Test case for facts and for pre-enumerated theory rows. Serializable so Test Explorer can run rows individually.</summary>
public sealed class RetryTestCase : XunitTestCase, ISelfExecutingXunitTestCase
{
    private const string MaxRetriesKey = nameof(MaxRetries);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public RetryTestCase()
    {
    }

    public RetryTestCase(
        int maxRetries,
        IXunitTestMethod testMethod,
        string testCaseDisplayName,
        string uniqueID,
        bool @explicit,
        string? testLabel,
        bool disableParallelization,
        Type[]? skipExceptions = null,
        string? skipReason = null,
        Type? skipType = null,
        string? skipUnless = null,
        string? skipWhen = null,
        Dictionary<string, HashSet<string>>? traits = null,
        object?[]? testMethodArguments = null,
        string? sourceFilePath = null,
        int? sourceLineNumber = null,
        int? timeout = null)
        : base(testMethod, testCaseDisplayName, uniqueID, @explicit, testLabel, disableParallelization, skipExceptions, skipReason, skipType, skipUnless, skipWhen, traits, testMethodArguments, sourceFilePath, sourceLineNumber, timeout)
    {
        MaxRetries = maxRetries;
    }

    public int MaxRetries { get; private set; }

    // 4.0 signature: ParallelMode + ExecutionScheduler are threaded through every runner.
    public ValueTask<RunSummary> Run(
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        object?[] constructorArguments,
        ExceptionAggregator aggregator,
        CancellationTokenSource cancellationTokenSource,
        ParallelMode parallelMode,
        ExecutionScheduler scheduler,
        FixtureMappingManager methodFixtureMappings) =>
        RetryTestCaseRunner.Instance.Run(
            MaxRetries,
            this,
            explicitOption,
            messageBus,
            aggregator.Clone(),
            TestCaseDisplayName,
            SkipReason,
            cancellationTokenSource,
            parallelMode,
            scheduler,
            constructorArguments,
            methodFixtureMappings);

    protected override void Deserialize(IXunitSerializationInfo info)
    {
        base.Deserialize(info);
        MaxRetries = info.GetValue<int>(MaxRetriesKey);
    }

    protected override void Serialize(IXunitSerializationInfo info)
    {
        base.Serialize(info);
        info.AddValue(MaxRetriesKey, MaxRetries);
    }
}
