using System.Diagnostics;
using Microsoft.Extensions.Logging.Abstractions;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace xunitv3.template.Examples.Telemetry;

[Collection(TelemetryCollection.Name)]
[TracedTest]
public sealed class TracesTests
{
    private const string OrderId = "order-1";

    private static readonly OrderService Service = new(NullLogger<OrderService>.Instance);

    [Fact]
    public void ActivityListenerObservesSpansWithoutTheOtelSdk()
    {
        var stopped = new List<Activity>();
        using var listener = new ActivityListener
        {
            ShouldListenTo = static source => source.Name == OrderService.SourceName,
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
            ActivityStopped = stopped.Add,
        };
        ActivitySource.AddActivityListener(listener);

        Service.Place(OrderId);

        var span = Assert.Single(stopped);
        Assert.Equal(OrderService.SpanName, span.OperationName);
        Assert.Equal(OrderId, span.GetTagItem(OrderService.OrderIdTag));
    }

    [Fact]
    public void InMemoryExporterCapturesSpansTaggedWithTheTestId()
    {
        var exported = new List<Activity>();
        using var provider = Sdk.CreateTracerProviderBuilder()
            .AddSource(OrderService.SourceName)
            .AddProcessor(new XunitTestIdProcessor())
            .AddInMemoryExporter(exported)
            .Build();

        Service.Place(OrderId);
        provider.ForceFlush();

        var span = Assert.Single(exported);
        Assert.Equal(TestContext.Current.Test!.UniqueID, span.GetTagItem(TracedTestAttribute.TestIdTag));
    }

    [Fact]
    public void TracedTestSpanIsTheParentOfSpansFromTheCodeUnderTest()
    {
        var testSpan = Activity.Current;
        var exported = new List<Activity>();
        using var provider = Sdk.CreateTracerProviderBuilder()
            .AddSource(OrderService.SourceName)
            .AddInMemoryExporter(exported)
            .Build();

        Service.Place(OrderId);
        provider.ForceFlush();

        Assert.NotNull(testSpan);
        Assert.Equal(TracedTestAttribute.SourceName, testSpan.Source.Name);
        Assert.Equal(TestContext.Current.Test!.TestDisplayName, testSpan.GetTagItem(TracedTestAttribute.TestCaseNameTag));
        Assert.Equal(testSpan.SpanId, Assert.Single(exported).ParentSpanId);
    }
}
