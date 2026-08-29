using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Testing;

namespace xunitv3.template.Examples.Telemetry;

public sealed class LogsTests(ITestOutputHelper output)
{
    private const string OrderId = "order-3";
    private const string OrderIdProperty = "OrderId";

    // Microsoft.Extensions.Diagnostics.Testing: assert on structured state, not on rendered text.
    [Fact]
    public void FakeLoggerRecordsStructuredState()
    {
        var logger = new FakeLogger<OrderService>();

        new OrderService(logger).Place(OrderId);

        var record = Assert.Single(logger.Collector.GetSnapshot());
        Assert.Equal(LogLevel.Information, record.Level);
        Assert.Contains(OrderId, record.Message);
        Assert.Equal(OrderId, record.StructuredState!.Single(static property => property.Key == OrderIdProperty).Value);
    }

    // ILogger -> ITestOutputHelper bridge: application logs show up under the test that caused them.
    [Fact]
    public void LoggerOutputIsBridgedIntoTheTestOutput()
    {
        using var factory = LoggerFactory.Create(builder => builder.AddProvider(new TestOutputLoggerProvider(output)));

        new OrderService(factory.CreateLogger<OrderService>()).Place(OrderId);

        Assert.Contains(OrderId, output.Output);
        Assert.Contains(typeof(OrderService).FullName!, output.Output);
    }
}
