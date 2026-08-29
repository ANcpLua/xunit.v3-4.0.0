using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using Xunit.v3;

namespace xunitv3.template.Examples.Telemetry;

/// <summary>
/// Gives every decorated test its own root span, so spans emitted by the code under test become its children.
/// xUnit.net has no built-in ActivitySource; a BeforeAfterTestAttribute is the hook for it.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Method)]
public sealed class TracedTestAttribute : BeforeAfterTestAttribute
{
    public const string SourceName = "xunit.v3.test";
    public const string TestCaseNameTag = "test.case.name";
    public const string TestIdTag = "xunit.test.id";

    private static readonly ActivitySource Source = new(SourceName);
    private static readonly ConcurrentDictionary<string, Activity> Spans = new(StringComparer.Ordinal);

    // An ActivitySource only produces spans when something listens; this listener guarantees the root span exists
    // even before an exporter is wired up. A TracerProvider that also calls AddSource(SourceName) exports them.
    private static readonly ActivityListener Listener = Listen();

    public override void Before(MethodInfo methodUnderTest, IXunitTest test)
    {
        var span = Source.StartActivity(test.TestDisplayName);
        if (span is null)
        {
            return;
        }

        span.SetTag(TestCaseNameTag, test.TestDisplayName);
        span.SetTag(TestIdTag, test.UniqueID);
        Spans[test.UniqueID] = span;
    }

    public override void After(MethodInfo methodUnderTest, IXunitTest test)
    {
        if (Spans.TryRemove(test.UniqueID, out var span))
        {
            span.Dispose();
        }
    }

    private static ActivityListener Listen()
    {
        var listener = new ActivityListener
        {
            ShouldListenTo = static source => source.Name == SourceName,
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
        };
        ActivitySource.AddActivityListener(listener);
        return listener;
    }
}

/// <summary>Stamps the running xunit test's unique id on every span, whatever its parent chain looks like.</summary>
public sealed class XunitTestIdProcessor : OpenTelemetry.BaseProcessor<Activity>
{
    public override void OnStart(Activity data)
    {
        if (TestContext.Current.Test is { } test)
        {
            data.SetTag(TracedTestAttribute.TestIdTag, test.UniqueID);
        }
    }
}
