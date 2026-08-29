using System.ComponentModel;
using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Retry;

/// <summary>Test case used when theory data is enumerated at execution time (pre-enumeration disabled or data not serializable).</summary>
public sealed class RetryDelayEnumeratedTestCase : XunitDelayEnumeratedTheoryTestCase, ISelfExecutingXunitTestCase
{
    private const string MaxRetriesKey = nameof(MaxRetries);

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
    public RetryDelayEnumeratedTestCase()
    {
    }

    public RetryDelayEnumeratedTestCase(
        int maxRetries,
        IXunitTestMethod testMethod,
        string testCaseDisplayName,
        string uniqueID,
        bool @explicit,
        bool skipTestWithoutData,
        Type[]? skipExceptions = null,
        string? skipReason = null,
        Type? skipType = null,
        string? skipUnless = null,
        string? skipWhen = null,
        Dictionary<string, HashSet<string>>? traits = null,
        string? sourceFilePath = null,
        int? sourceLineNumber = null,
        int? timeout = null)
        : base(testMethod, testCaseDisplayName, uniqueID, @explicit, skipTestWithoutData, skipExceptions, skipReason, skipType, skipUnless, skipWhen, traits, sourceFilePath, sourceLineNumber, timeout)
    {
        MaxRetries = maxRetries;
    }

    public int MaxRetries { get; private set; }

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
