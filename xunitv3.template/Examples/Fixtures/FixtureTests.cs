namespace xunitv3.template.Examples.Fixtures;

public sealed class FixtureTests(TelemetryFixture telemetry)
{
    [Fact]
    public void AssemblyFixtureIsASingleton() => Assert.Equal(1, TelemetryFixture.Instances);

    [Fact]
    public void AssemblyFixtureIsInitializedBeforeTests() => Assert.True(telemetry.Initialized);

    [Fact]
    public void AssemblyFixtureSawTheAssemblyStart() => Assert.True(telemetry.AssemblyStarted);

    [Fact]
    public void FixtureObservesTheRunningTest()
    {
        var displayName = TestContext.Current.Test!.TestDisplayName;

        Assert.Contains(displayName, telemetry.Started);
        Assert.DoesNotContain(displayName, telemetry.Finished);
    }
}
