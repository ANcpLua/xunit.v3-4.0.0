using Xunit.Sdk;
using Xunit.v3;

namespace xunitv3.template.Examples.Retry;

public sealed class RetryTestCaseRunner : XunitTestCaseRunnerBase<RetryTestCaseRunnerContext, IXunitTestCase, IXunitTest>
{
    private const string RetryDiagnosticFormat = "Execution of '{0}' failed (attempt #{1}), retrying...";

    public static RetryTestCaseRunner Instance { get; } = new();

    public async ValueTask<RunSummary> Run(
        int maxRetries,
        IXunitTestCase testCase,
        ExplicitOption explicitOption,
        IMessageBus messageBus,
        ExceptionAggregator aggregator,
        string displayName,
        string? skipReason,
        CancellationTokenSource cancellationTokenSource,
        ParallelMode parallelMode,
        ExecutionScheduler scheduler,
        object?[] constructorArguments,
        FixtureMappingManager methodFixtureMappings)
    {
        // Mirrors XunitRunnerHelper.RunXunitTestCase so the two test case types share one implementation.
        var tests = await aggregator.RunAsync(testCase.CreateTests, []);

        if (aggregator.ToException() is { } exception)
        {
            return exception.Message.StartsWith(DynamicSkipToken.Value, StringComparison.Ordinal)
                ? XunitRunnerHelper.SkipTestCases(messageBus, cancellationTokenSource, [testCase], exception.Message[DynamicSkipToken.Value.Length..], sendTestCaseMessages: false)
                : XunitRunnerHelper.FailTestCases(messageBus, cancellationTokenSource, [testCase], exception, sendTestCaseMessages: false);
        }

        await using var context = new RetryTestCaseRunnerContext(
            maxRetries,
            testCase,
            tests,
            explicitOption,
            messageBus,
            aggregator,
            displayName,
            skipReason,
            cancellationTokenSource,
            parallelMode,
            scheduler,
            constructorArguments,
            methodFixtureMappings);
        await context.InitializeAsync();

        return await Run(context);
    }

    protected override async ValueTask<RunSummary> RunTest(RetryTestCaseRunnerContext context, IXunitTest test)
    {
        var attempt = 0;
        var maxRetries = Math.Max(context.MaxRetries, 1);

        while (true)
        {
            var delayedMessageBus = new DelayedMessageBus(context.MessageBus);
            var aggregator = context.Aggregator.Clone();
            var result = await XunitTestRunner.Instance.Run(
                test,
                delayedMessageBus,
                context.ConstructorArguments,
                context.ExplicitOption,
                aggregator,
                context.CancellationTokenSource,
                context.ParallelMode,
                context.Scheduler,
                context.BeforeAfterTestAttributes,
                context.CaseFixtureMappings);

            var failed = aggregator.HasExceptions || result.Failed != 0;
            if (!failed || ++attempt >= maxRetries)
            {
                delayedMessageBus.Dispose();
                return result;
            }

            TestContext.Current.SendDiagnosticMessage(RetryDiagnosticFormat, test.TestDisplayName, attempt);
            context.Aggregator.Clear();
        }
    }
}

public sealed class RetryTestCaseRunnerContext(
    int maxRetries,
    IXunitTestCase testCase,
    IReadOnlyCollection<IXunitTest> tests,
    ExplicitOption explicitOption,
    IMessageBus messageBus,
    ExceptionAggregator aggregator,
    string displayName,
    string? skipReason,
    CancellationTokenSource cancellationTokenSource,
    ParallelMode parallelMode,
    ExecutionScheduler scheduler,
    object?[] constructorArguments,
    FixtureMappingManager methodFixtureMappings)
    : XunitTestCaseRunnerBaseContext<IXunitTestCase, IXunitTest>(testCase, tests, explicitOption, messageBus, aggregator, displayName, skipReason, cancellationTokenSource, parallelMode, scheduler, constructorArguments, methodFixtureMappings)
{
    public int MaxRetries { get; } = maxRetries;
}
