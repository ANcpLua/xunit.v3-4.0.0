using System.Globalization;
using Microsoft.Extensions.Logging;

namespace xunitv3.template.Examples.Telemetry;

/// <summary>
/// ILoggerProvider that writes into the current test's ITestOutputHelper. Same shape as the provider the
/// modelcontextprotocol/csharp-sdk test suite uses with xunit.v3 (tests/Common/Utils/XunitLoggerProvider.cs).
/// </summary>
public sealed class TestOutputLoggerProvider(ITestOutputHelper output) : ILoggerProvider
{
    private const string LineFormat = "[{0}] {1} {2}: {3}";

    public ILogger CreateLogger(string categoryName) => new TestOutputLogger(output, categoryName);

    public void Dispose()
    {
    }

    private sealed class TestOutputLogger(ITestOutputHelper output, string category) : ILogger
    {
        public IDisposable? BeginScope<TState>(TState state)
            where TState : notnull => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                output.WriteLine(string.Format(CultureInfo.InvariantCulture, LineFormat, DateTimeOffset.UtcNow, category, logLevel, formatter(state, exception)));
            }
            catch (InvalidOperationException)
            {
                // Background work may log after the test finished and its output helper was torn down.
            }
        }
    }
}
