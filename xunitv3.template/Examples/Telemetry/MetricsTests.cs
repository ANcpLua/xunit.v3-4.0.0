using Microsoft.Extensions.Diagnostics.Metrics.Testing;
using Microsoft.Extensions.Logging.Abstractions;
using OpenTelemetry;
using OpenTelemetry.Metrics;

namespace xunitv3.template.Examples.Telemetry;

[Collection(TelemetryCollection.Name)]
public sealed class MetricsTests
{
    private const string OrderId = "order-2";
    private const int Placed = 3;

    private static readonly OrderService Service = new(NullLogger<OrderService>.Instance);

    // Microsoft.Extensions.Diagnostics.Testing: attach to one instrument and read raw measurements with tags.
    [Fact]
    public void MetricCollectorRecordsEveryMeasurementWithItsTags()
    {
        using var collector = new MetricCollector<long>(OrderService.Meter, OrderService.OrdersPlacedName);

        for (var i = 0; i < Placed; i++)
        {
            Service.Place(OrderId);
        }

        var measurements = collector.GetMeasurementSnapshot();
        Assert.Equal(Placed, measurements.Count);
        Assert.All(measurements, static measurement => Assert.Equal(1, measurement.Value), throwIfEmpty: true);
        Assert.Equal(OrderId, collector.LastMeasurement!.Tags[OrderService.OrderIdTag]);
    }

    // OpenTelemetry SDK: aggregate through a MeterProvider and read the exported metric points.
    [Fact]
    public void InMemoryMetricExporterAggregatesTheCounter()
    {
        var exported = new List<Metric>();
        using var provider = Sdk.CreateMeterProviderBuilder()
            .AddMeter(OrderService.SourceName)
            .AddInMemoryExporter(exported)
            .Build();

        for (var i = 0; i < Placed; i++)
        {
            Service.Place(OrderId);
        }

        provider.ForceFlush();

        var metric = Assert.Single(exported, static metric => metric.Name == OrderService.OrdersPlacedName);
        long sum = 0;
        foreach (ref readonly var point in metric.GetMetricPoints())
        {
            sum += point.GetSumLong();
        }

        Assert.Equal(Placed, sum);
    }
}
